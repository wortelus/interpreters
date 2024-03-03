from enum import Enum

global unknown_kw

TokenType = Enum("TYPE", ["NUM",
                          "OP",
                          "LPAR",
                          "RPAR",
                          "SEMICOLON",
                          "DIV",
                          "MOD",
                          "ID",
                          ])


class Token:
    def __init__(self, type, value):
        self.type = TokenType[type]
        self.value = value

    def __str__(self):
        if self.value is not None:
            return f"{self.type}:{self.value}"
        else:
            return f"{self.type}"

    def __repr__(self):
        return self.__str__()


def tokenize(expression):
    tokens = []

    number = ''
    cmd = 0

    global unknown_kw
    unknown_kw = ""
    keywords = {
        "div": 0,
        "mod": 0,
    }

    def reset_keywords():
        global unknown_kw
        if unknown_kw != "" and unknown_kw not in keywords.keys():
            tokens.append(Token('ID', unknown_kw))
            unknown_kw = ""
        for k in keywords.keys():
            keywords[k] = 0

    for char in expression:

        # Handle comments
        if cmd == 2:
            if char == '\n':
                cmd = 0
            continue
        elif char == '/':
            if cmd == 1:
                cmd += 1
                tokens.pop()
                continue
            else:
                cmd = 1
        else:
            cmd = 0

        # Handle tokens
        if char.isdigit():
            # reset keywords
            reset_keywords()
            number += char
        elif char == ' ' or char == '\n':
            # reset keywords
            reset_keywords()
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
                tokens.append(Token('LPAR', None))
            elif char == ')':
                tokens.append(Token('RPAR', None))
            elif char in "+-*":
                tokens.append(Token('OP', char))
            elif char == '/':
                tokens.append(Token('DIV', None))
            # Handle multiple chars
            else:
                unknown_kw += char
                for keyword in keywords:
                    if keyword.startswith(char[keywords[keyword]:]):
                        keywords[keyword] += 1
                        if keywords[keyword] == len(keyword):
                            tokens.append(Token(keyword.upper(), None))
                            unknown_kw = ""
                            keywords[keyword] = 0
                    else:
                        keywords[keyword] = 0

    if number:
        tokens.append(int(number))
    return tokens


expr = ""
while input_str := input(">"):
    expr += input_str + "\n"

tokens = tokenize(expr)
for token in tokens:
    print(token)
