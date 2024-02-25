from expression_interpreter import evaluate_expression

inputs = [
    "10",
    "3 + 4 * 2",
    "(1 + 2) * 3",
    "7 - 3 * (10 / (12 / (3 + 1) - 1))",
    "7 - (-3) * (10 / (12 / (3 + 1) - 1))",
    "-3 * (10 / (12 / (3 + 1) - 1))",
    "3 + 4 * 2 / (1 - 5) ** 2 ** 3",
    "3 + 4 * 2 / (1 - 5) * 2 * 3",
]

outputs = [
    "10",
    "11",
    "9",
    "-8",
    "ERROR",
    "ERROR",
    "ERROR",
    "-9"
]

for i, expr in enumerate(inputs):
    if (result := str(evaluate_expression(expr))) != outputs[i]:
        print(f"ANGERY!!!! {i} failed: expected {outputs[i]}, got {result}")
    else:
        print(f"{result} == {outputs[i]}")
