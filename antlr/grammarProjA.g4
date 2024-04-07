grammar grammarProjA;

program: statement+;

statement:
	TYPE_KEYWORD IDENTIFIER (COMMA IDENTIFIER)* SEMICOLON              # declarations
	| '{' statement* '}'                                               # block
	| WHILE '(' expression ')' statement                               # while
	| IF '(' expression ')' iftrue=statement (ELSE ifelse=statement)?  # ifElse
	| READ IDENTIFIER (COMMA IDENTIFIER)* SEMICOLON                    # read
	| WRITE expression (COMMA expression)* SEMICOLON                   # write
	| expression SEMICOLON                                             # eval
	| SEMICOLON                                                        # emptyStatement
	;


expression:
    '('expression')'                                 # exprParentheses
	| IDENTIFIER                                     # identifier
	| INT                                            # int
	| FLOAT                                          # float
	| BOOL                                           # bool
	| STRING                                         # string
	| expression op=(ADD | SUB) expression           # addSubOp
	| expression op=(MUL | DIV) expression           # mulDivOp
	| prefix=(SUB | NEG) expression                  # prefix
	| expression op=(LT | EQ | NEQ | GT) expression  # arithmeticComp
	| expression op=(AND | OR) expression            # logicalComp
	;

assignment:
    Identifier ASSIGN assignment    // recursive rule for right-associativity
    | expression                 // fallback to other expression types
    ;


TYPE_KEYWORD:
	INT_KEYWORD
	| FLOAT_KEYWORD
	| BOOL_KEYWORD
	| STRING_KEYWORD
	;

// identifier

IDENTIFIER: [a-zA-Z] [a-zA-Z0-9]* ;

// types

INT: [0-9]+ ;
FLOAT: [0-9]+.[0-9]+ ;
BOOL: 'true' | 'false' ;
STRING: [a-zA-Z0-9]* ;

// const symbols
ASSIGN: '=' ;
ADD: '+' ;
SUB: '-' ;
MUL: '*' ;
DIV: '/' ;
MOD: '%' ;
CONCAT: '.' ;
LT: '<' ;
// LTE: '<=' ;
EQ: '==' ;
NEQ: '!=' ;
GT: '>' ;
// GTE: '>=' ;
AND: '&&' ;
OR: '||' ;
NEG: '!' ;

INT_KEYWORD: 'int' ;
FLOAT_KEYWORD: 'float' ;
BOOL_KEYWORD: 'bool' ;
STRING_KEYWORD: 'string' ;

WHILE: 'while' ;
IF: 'if' ;
ELSE: 'else' ;

WRITE: 'write' ;
READ: 'read' ;

COMMA: ',' ;
SEMICOLON: ';';