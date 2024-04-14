grammar grammarProjA;

program: (statement)*;

TYPE_KEYWORD:
	INT_KEYWORD
	| FLOAT_KEYWORD
	| BOOL_KEYWORD
	| STRING_KEYWORD;

statement:
	SEMICOLON											# emptyStatement
	| TYPE_KEYWORD IDENTIFIER (',' IDENTIFIER)* ';'		# declarations
	| READ IDENTIFIER (COMMA IDENTIFIER)* SEMICOLON		# read
	| WRITE expression (COMMA expression)* SEMICOLON	# write
	| '{' (statement)+ '}'								# block
	| IF '(' expression ')' iftrue = statement (
		ELSE ifelse = statement
	)?										            # ifElse
	| WHILE '(' expression ')' statement	            # while
	| DO statement WHILE '(' expression ')' SEMICOLON   # doWhile
	| expression SEMICOLON					            # eval
	;

expression:
	'(' expression ')'									# exprParentheses
	| prefix = SUB expression							# prefixSub
	| prefix = NEG expression							# prefixNeg
	| expression op = (MUL | DIV | MOD) expression		# mulDivOp
	| expression op = (ADD | SUB | CONCAT) expression	# addSubOp
	| expression op = (LT | GT) expression				# arithmeticComp
	| expression op = (EQ | NEQ) expression				# arithmeticEq
	| expression op = AND expression					# logicalAnd
	| expression op = OR expression						# logicalOr
	| <assoc = right> IDENTIFIER '=' expression			# assignment
	| INT												# int
	| FLOAT												# float
	| BOOL												# bool
	| IDENTIFIER										# identifier
	| STRING											# string
	;

INT_KEYWORD: 'int';
FLOAT_KEYWORD: 'float';
BOOL_KEYWORD: 'bool';
STRING_KEYWORD: 'string';
WHILE: 'while';
DO: 'do' ;
IF: 'if';
ELSE: 'else';
WRITE: 'write';
READ: 'read';

// types
BOOL: 'true' | 'false';
// identifier
IDENTIFIER: [a-zA-Z]+;
INT: [0-9]+;
FLOAT: [0-9]* '.' [0-9]+;

STRING: '"' ( ~["\\] | ESCAPE_SEQ)* '"';

fragment ESCAPE_SEQ:
	'\\' '"'
	| '\\' '\\'
	| '\\' 'r' '\\' 'n'
	| '\\' 'n'
	| '\\' 't'
	;

// const symbols
ASSIGN: '=';
ADD: '+';
SUB: '-';
MUL: '*';
DIV: '/';
MOD: '%';
CONCAT: '.';
LT: '<';
EQ: '==';
NEQ: '!=';
GT: '>';
AND: '&&';
OR: '||';
NEG: '!';

COMMA: ',';
SEMICOLON: ';';

WS: [ \t\r\n]+ -> skip;
COMMENT: '/*' .*? '*/' -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;