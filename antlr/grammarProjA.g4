grammar grammarProjA;

program: statement+ ;

statement
    : '{' statement (statement)* '}'                                   # block
    | typedef IDENTIFIER ( COMMA IDENTIFIER)* SEMICOLON                # declaration
    | IF '(' expression ')' iftrue=statement (ELSE iffalse=statement)? # ifElse
    | WHILE '(' expression ')' statement                               # while
    | READ IDENTIFIER ( COMMA IDENTIFIER)* SEMICOLON                   # readStatement
    | WRITE expression ( COMMA expression)* SEMICOLON                  # writeStatement
    | expression SEMICOLON                                             # eval
    | SEMICOLON                                                        # emptyStatement
    ;



expression: 
    IDENTIFIER                                        # id
    | ('true'|'false')                                # bool
    | '(' expression ')'                              # parens
    | INT                                             # int
    | FLOAT                                           # float
    | STRING                                          # string
    | prefix=SUB expression                           # unarySub
    | prefix=NOT expression                           # unaryNot
    | expression op=(MUL|DIV|MOD) expression          # arithmeticMulDivMod
    | expression op=(ADD|SUB|CONCAT) expression       # arithmeticAddSubConcat
    | expression op=(LT|GT) expression                # comparison
    | expression op=(EQ|NEQ) expression               # equals
    | expression AND expression                       # logicalAnd
    | expression OR expression                        # logicalOr
    | <assoc=right> IDENTIFIER '=' expression         # assignment
    ;

typedef
    : type=INT_KEYWORD
    | type=FLOAT_KEYWORD
    | type=STRING_KEYWORD
    | type=BOOL_KEYWORD
    ;


INT_KEYWORD : 'int';
FLOAT_KEYWORD : 'float';
STRING_KEYWORD : 'string';
BOOL_KEYWORD : 'bool';

// const symbols

MUL : '*' ;
DIV : '/' ;
MOD : '%' ;
ADD : '+' ;
SUB : '-' ;
LT : '<' ;
EQ  : '==' ;
NEQ : '!=' ;
GT : '>' ;
NOT : '!' ;
AND : '&&' ;
OR : '||' ;
CONCAT : '.' ;

SEMICOLON:  ';';
COMMA: ',';

READ : 'read' ;
WRITE : 'write' ;
IF : 'if' ;
ELSE : 'else' ;
WHILE : 'while' ;

IDENTIFIER : [a-zA-Z] [a-zA-Z0-9]* ;

// DATA TYPES

FLOAT : [0-9]+'.'[0-9]+ ;
INT : [0-9]+ ;
BOOL : 'true' | 'false' ;
STRING : '"' (~["\\\r\n] | STRING_ESC)* '"';

fragment STRING_ESC: '\\' [btnfr"'\\\r\n] ;

WS : [ \t\r\n]+ -> skip ;
COMMENT: '/*' .*? '*/' -> skip ;
LINE_COMMENT: '//' ~[\r\n]* -> skip ;