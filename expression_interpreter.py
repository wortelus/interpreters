def tokenize(expression):
    tokens = []
    number = ''
    for char in expression:
        if char.isdigit():
            number += char
        elif char == ' ':
            continue
        else:
            if number:
                tokens.append(int(number))
                number = ''
            if char in "+-*/()":
                tokens.append(char)
    if number:
        tokens.append(int(number))
    return tokens


def to_rpn(tokens):
    # Operator precedence
    precedence = {'+': 1, '-': 1, '*': 2, '/': 2}

    # Output queue and operator stack
    output_queue = []
    operator_stack = []

    # Shunting-yard algorithm
    for token in tokens:
        #
        # If the token is a number, add it to the output queue
        #
        if isinstance(token, int):
            output_queue.append(token)

        #
        # If the token is an operator
        #
        elif token in "+-*/":
            #
            # Pop operators from the stack to the output queue
            #
            while operator_stack and \
                    operator_stack[-1] in precedence and \
                    precedence[operator_stack[-1]] >= precedence[token]:
                # 1) stack not empty
                # 2) top of the stack is an operator
                # 3) precedence of the top operator >= precedence of the token

                # Pop operator from the stack to the output queue
                output_queue.append(operator_stack.pop())
            # Finally, push the token to the stack
            operator_stack.append(token)

        # Handle parentheses
        elif token == '(':
            operator_stack.append(token)
        elif token == ')':
            top_token = operator_stack.pop()
            while top_token != '(':
                output_queue.append(top_token)
                top_token = operator_stack.pop()

    # Pop remaining operators from the stack
    while operator_stack:
        output_queue.append(operator_stack.pop())
    return output_queue


def evaluate(rpn):
    stack = []
    for token in rpn:
        if isinstance(token, int):
            stack.append(token)
        else:
            try:
                right = stack.pop()
                left = stack.pop()
            except IndexError:
                return "ERROR"
            if token == '+':
                stack.append(left + right)
            elif token == '-':
                stack.append(left - right)
            elif token == '*':
                stack.append(left * right)
            elif token == '/':
                if right == 0:
                    return "ERROR"
                stack.append(left // right)
    return stack[0]


def evaluate_expression(expression):
    tokens = tokenize(expression)
    try:
        rpn = to_rpn(tokens)
    except IndexError:
        return "ERROR"
    result = evaluate(rpn)
    return result


def main():
    count = int(input("Počet výrazů: "))
    results = []
    for i in range(count):
        expr = input(f"{i}>").replace(' ', '')
        result = evaluate_expression(expr)
        results.append(result)

    for result in results:
        print(result)


if __name__ == "__main__":
    main()
