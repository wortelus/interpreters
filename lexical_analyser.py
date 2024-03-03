from enum import Enum

TokenType = Enum("TYPE", ["NUM",
                          "OP",
                          "LPAR",
                          "RPAR",
                          "SEMICOLON",
                          "DIV",
                          "MOD",
                          ])


class Token:
    def __init__(self, type, value):
        self.type = TokenType[type]
        self.value = value

    def __str__(self):
        return f"{self.type}:{self.value}"

    def __repr__(self):
        return self.__str__()


def tokenize(expression):
    tokens = []

    number = ''
    cmd = 0
    keywords = {
        "div": 0,
        "mod": 0,
    }
    for char in expression:

        # Handle comments
        if cmd == 2:
            if char == '\n':
                cmd = 0
            continue
        elif char == '/':
            if cmd == 1:
                cmd += 1
            else:
                cmd = 1
        else:
            cmd = 0

        # Handle tokens
        if char.isdigit():
            number += char
        elif char == ' ':
            continue
        else:
            # Handle number construction
            if number:
                tokens.append(Token('NUM', int(number)))
                number = ''
            # Handle single chars
            if char == ';':
                tokens.append(Token('SEMICOLON', char))
            if char == '(':
                tokens.append(Token('LPAR', char))
            elif char == ')':
                tokens.append(Token('RPAR', char))
            elif char in "+-*":
                tokens.append(Token('OP', char))
            elif char == '/':
                tokens.append(Token('DIV', char))
            # Handle multiple chars
            else:
                for keyword in keywords:
                    if keyword.startswith(char[keywords[keyword]:]):
                        keywords[keyword] += 1
                        if keywords[keyword] == len(keyword):
                            tokens.append(Token(keyword.upper(), keyword))
                            keywords[keyword] = 0
                    else:
                        keywords[keyword] = 0

    if number:
        tokens.append(int(number))
    return tokens


input_str = input("Zadej výraz: \n")
tokens = tokenize(input_str)
for token in tokens:
    print(token)
