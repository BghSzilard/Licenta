from googletrans import Translator

def translate_to_english(text):
    translator = Translator()
    
    detected_language = translator.detect(text).lang
    if detected_language == 'en':
        return text
    
    translated_text = translator.translate(text, dest='en')
    return translated_text.text

if __name__ == "__main__":
    text = input()
    translated_text = translate_to_english(text)
    print(translated_text)