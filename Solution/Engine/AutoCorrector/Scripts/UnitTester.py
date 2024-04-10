import argparse
import shlex
import sys
import subprocess

sys.path.append('C:\\Users\\z004w26z\\Desktop\\Module\\build\\Debug')

def parse_arguments(argument):
    argument_list = shlex.split(argument)

    argument_list = [arg for arg in argument_list if arg != '-']

    parser = argparse.ArgumentParser()

    parser.add_argument("pairs", nargs="*")

    args = parser.parse_args(argument_list)

    pairs_dict = dict(zip(args.pairs[::2], args.pairs[1::2]))

    dict_without_dollars = {k.replace('$', ''): v.replace('$', '') for k, v in pairs_dict.items()}

    return dict_without_dollars

def create_library(function_name, include_header):
    file_path = r"C:\Users\z004w26z\Desktop\Module\main.cpp"
    pybind_code = f'''
    #include <pybind11/pybind11.h>
    #include <pybind11/stl.h>
    #include "python3.11/Python.h"

    #include "{include_header}"

    namespace py = pybind11;

    PYBIND11_MODULE(mylibrary, m) {{
        m.def("test", &{function_name});
    }}
    '''

    with open(file_path, 'w') as file:
        file.write(pybind_code)

def compile_library():
    command = 'msbuild "C:\\Users\\z004w26z\\Desktop\\Module\\build\\mylibrary.sln"'
    process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    process.communicate()

def convert_to_numerical_or_boolean(arg):
    if arg.lower() in ('true', 'false'):
        return arg.lower() == 'true'
    if ' ' in arg:
        return [int(num) for num in arg.split()]
    try:
        return int(arg)
    except ValueError:
        try:
            return float(arg)
        except ValueError:
            return arg

arg1 = sys.argv[1]
functionName = sys.argv[2]
include_header = sys.argv[3]

# arg1 = '$5$ $1200$'
# functionName = "factorial"
# include_header = "C:\\Users\\z004w26z\\Desktop\\asd.h"

create_library(functionName, include_header)
compile_library()
success = True
import mylibrary

# print(arg1)

for key, value in parse_arguments(arg1).items():
    new_key = convert_to_numerical_or_boolean(key)
    new_value = convert_to_numerical_or_boolean(value)
    result = mylibrary.test(new_key)
    print("Input: " + str(new_key) + " Expected Output: " + str(new_value) + " Actual Output: " + str(result))
    if result != new_value:
        print("Fail!")
        success = False
        break
if success == True:
    print("Success!")