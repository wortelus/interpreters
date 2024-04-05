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
                          "EOL"
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


class Tokenizer(object):
    def __init__(self, expr):
        self.cmd = 0
        self.number = ''
        self.tokens = []

        self.unknown_kw = ""
        self.keywords = {
            "div": 0,
            "mod": 0,
        }

        self.i = 0
        self.expr = expr

    def reset_keywords(self):
        if self.unknown_kw != "" and self.unknown_kw not in self.keywords.keys():
            tokens.append(Token('ID', self.unknown_kw))
            self.unknown_kw = ""
        for k in self.keywords.keys():
            self.keywords[k] = 0

    def next(self):
        if self.i >= len(self.expr):
            return Token('EOL', None)

        char = self.expr[self.i]

        if self.cmd == 2:
            if char == '\n':
                self.cmd = 0
            return
        elif char == '/':
            if self.cmd == 1:
                self.cmd += 1
                tokens.pop()
                return
            else:
                self.cmd = 1
        else:
            self.cmd = 0

        # Handle tokens
        if char.isdigit():
            # reset keywords
            self.reset_keywords()
            self.number += char
        elif char == ' ' or char == '\n':
            # reset keywords
            self.reset_keywords()
            return
        else:
            # Handle number construction
            if self.number:
                tokens.append(Token('NUM', int(self.number)))
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
                self.unknown_kw += char
                for keyword in self.keywords:
                    if keyword.startswith(char[self.keywords[keyword]:]):
                        self.keywords[keyword] += 1
                        if self.keywords[keyword] == len(keyword):
                            tokens.append(Token(keyword.upper(), None))
                            unknown_kw = ""
                            self.keywords[keyword] = 0
                    else:
                        self.keywords[keyword] = 0


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

    if number:
        tokens.append(int(number))
    return tokens


expr = ""
while input_str := input(">"):
    expr += input_str + "\n"

tokens = tokenize(expr)
for token in tokens:
    print(token)
