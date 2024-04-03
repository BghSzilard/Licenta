import numpy as np
from gensim.models import KeyedVectors
import spacy
from collections import Counter
import sys

# Load pre-trained word embeddings
#word_vectors = KeyedVectors.load_word2vec_format("C:\\Users\\sziba\\Desktop\\GoogleNews-vectors-negative300.bin", binary=True, limit=300000)


# def calculate_similarity(word1, word2):
#     if word1 in word_vectors and word2 in word_vectors:
#         # Retrieve word vectors
#         vec1 = word_vectors[word1]
#         vec2 = word_vectors[word2]
       
#         # Calculate cosine similarity
#         similarity = np.dot(vec1, vec2) / (np.linalg.norm(vec1) * np.linalg.norm(vec2))
#         return similarity
#     else:
#         return 0  # If any of the words is not in vocabulary, return 0 similarity


# Define numerical types
numerical_types = ["int", "long", "float", "double"]


# Define container types
container_types = ["vector", "array", "set"]


# Generate implicit casts for numerical types
implicit_casts_numerical = {num_type: [other_num_type for other_num_type in numerical_types if other_num_type != num_type] for num_type in numerical_types}


# Generate implicit casts for container types
implicit_casts_containers = {}
for num_type in numerical_types:
    for container in container_types:
        key = f"{container}<{num_type}>"
        implicit_casts_containers[key] = [f"{other_container}<{other_num_type}>" for other_container in container_types for other_num_type in numerical_types if other_container != container or other_num_type != num_type]
        # Add mapping from container<num_type> to all num_type[]
        implicit_casts_containers[key].extend([f"{other_num_type}[]" for other_num_type in numerical_types])
    for num_type in numerical_types:
        key = f"{num_type}[]"
        implicit_casts_containers[key] = [f"{other_container}<{other_num_type}>" for other_container in container_types for other_num_type in numerical_types]
        # Add mapping from num_type[] to all other num_type[]
        implicit_casts_containers[key].extend([f"{other_num_type}[]" for other_num_type in numerical_types if other_num_type != num_type])


# Combine numerical and container casts
implicit_casts = {**implicit_casts_numerical, **implicit_casts_containers}


def get_implicit_castable_types(cpp_type):
    if cpp_type in implicit_casts:
        return implicit_casts[cpp_type]
    else:
        return []


def parse_cpp_function_signature(signature):
        # Split the signature into return type, function name, and parameters
        return_type, rest = signature.split(' ', 1)
        function_name, params = rest.split('(', 1)
        params = params.rstrip(')')  # Remove the closing parenthesis

        # Split the parameters into a list, handling the case of no parameters
        param_types = params.split(', ') if params else []

        # Create and return the Function object
        return Function(param_types, return_type, function_name)

def parse_function_string(function_string):
    # Split the string into arguments and return type
    args_and_return = function_string.split(' -> ')
    
    # Remove the curly braces and split the arguments by comma
    arguments = args_and_return[0].strip('{}').split(', ')
    
    # The return type is the second part of the split string
    return_type = args_and_return[1]
    
    # Create a Function object and return it
    return Function(arguments, return_type)

class Function:
    def __init__(self, param_types, return_type, function_name=None):
        self.param_types = param_types
        self.return_type = return_type
        self.function_name = function_name


# # Load the spacy model
# nlp = spacy.load('en_core_web_lg')


# Load the Word2Vec model

def get_centroid_vector(keywords, word_vectors):
    vectors = []
    for word in keywords:
        if word in word_vectors:
            vectors.append(word_vectors[word])
    if vectors:
        return np.mean(vectors, axis=0)
    else:
        return np.zeros(word_vectors.vector_size)  # return zero vector if no valid words




def find_match(function_A, function_list, threshold):
    perfect_matches = []
    acceptable_matches = []
   
    for function in function_list:
        # Check for perfect match
        if Counter(function_A.param_types) == Counter(function.param_types) and function_A.return_type == function.return_type:
            perfect_matches.append(function)
        else:
            # Check for acceptable substitutions
            param_types_A_casts = [get_implicit_castable_types(param_type_A) for param_type_A in function_A.param_types]
            if all(any(param_type in param_types_A_cast for param_type in function.param_types) for param_types_A_cast in param_types_A_casts) and function_A.return_type == function.return_type:
                acceptable_matches.append(function)
   
    if perfect_matches:
        if len(perfect_matches) == 1:
            return perfect_matches[0]
        else:
            word_vectors = KeyedVectors.load_word2vec_format("C:\\Users\\z004w26z\\Desktop\\Material\\GoogleNews-vectors-negative300.bin", binary=True, limit=200000)
            # Tokenize function names
            function_A_name_tokens = function_A.function_name.split('_')
            function_names_tokens = [f.function_name.split('_') for f in perfect_matches]
           
            # Calculate centroid vectors
            function_A_vector = get_centroid_vector(function_A_name_tokens, word_vectors)
            function_vectors = [get_centroid_vector(tokens, word_vectors) for tokens in function_names_tokens]
           
            # Calculate cosine similarities
            cos_similarities = [np.dot(function_A_vector, v) / (np.linalg.norm(function_A_vector) * np.linalg.norm(v)) for v in function_vectors]
           
            # Return the function with the highest cosine similarity
            return perfect_matches[np.argmax(cos_similarities)]
   
    # If no perfect matches, check for acceptable matches
    if acceptable_matches:
        # Tokenize function names
        function_A_name_tokens = function_A.function_name.split('_')
        function_names_tokens = [f.function_name.split('_') for f in acceptable_matches]
       
        # Calculate centroid vectors
        function_A_vector = get_centroid_vector(function_A_name_tokens, word_vectors)
        function_vectors = [get_centroid_vector(tokens, word_vectors) for tokens in function_names_tokens]
       
        # Calculate cosine similarities
        cos_similarities = [np.dot(function_A_vector, v) / (np.linalg.norm(function_A_vector) * np.linalg.norm(v)) for v in function_vectors]
       
        # Filter matches based on cosine similarity
        matches = [f for f, sim in zip(acceptable_matches, cos_similarities) if sim > threshold]
       
        if matches:
            # Return the function with the highest cosine similarity
            return matches[np.argmax(cos_similarities)]
   
    return None

def remove_additional(str):
    str = str.replace("const ", "")
    str = str.replace("&", "")
    return str.replace("std::", "")

# def get_most_similar(function_A, functionSignatures):
#     functions = []
#     for functionSignature in functionSignatures:
#         functions.append(parse_cpp_function_signature(functionSignature))
#     function_A = parse_function_string(function_A)
#     matching_function = find_match(function_A, functions, 0)
#     return matching_function

# Example usage
#similarity_number_integer = calculate_similarity("integer", "dog")


# print("Similarity between 'print' and 'display':", similarity_number_integer)


# functions = [
#     Function(["vector<int>"], "int", "get_random"),
#     Function(["vector<int>"], "float", "calculate_average"),
#     Function(["float", "int"], "float", "compute_mean"),
#     Function(["int", "int"], "float", "add_numbers")
# ]

# # Example function for which we want to find a match
# input_function = Function(["vector<int>"], "float", "find_average")


# Find matching function
# matching_function = find_match(input_function, functions, 0)
# print("Matching function:", matching_function.function_name if matching_function else "No match found")

# print(parse_cpp_function_signature('int findSum(int, int)'))
# print(parse_cpp_function_signature('int calculateAverage(float, double, double)'))
# print(parse_cpp_function_signature('int main()'))

# print("hello boszomeg")



functions = sys.argv[1]
function_A = sys.argv[2]
# functions = "int getAverage(std::vector<int>);std::vector<int> bubbleSort(std::vector<int>);bool isPrime(int);int factorial(int);int fibonacci(int);int main()"
# function_A = "{int} -> int"
function_A = remove_additional(function_A)
function_A = parse_function_string(function_A)
functions = remove_additional(functions)
functions = functions.split(';')
functions = [parse_cpp_function_signature(function) for function in functions]
matching_function = find_match(function_A, functions, 0)
if hasattr(matching_function, 'function_name'):
    print(matching_function.function_name)
else:
    print("No function could be found!")