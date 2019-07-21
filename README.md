# Unity Advanced Duplicator

Easy to use duplicator for any type of game object within Unity, allows for adding offsets using formulas such as (w - n * 3) which would just be the width of the object - the current duplicate number * 3

## Features

### Linear Duplication
Basically just duplication on 1 axis </br>
Duplicate Amount: Number of dupes.. duh?

Offset variables:
| Var Name | Description |
| -------- | ----------- |
| N | Current dupe index |
| W | Width of object |
| H | Height of object |

![Linear Preview](linearexample_preview.png) ![Linear Values](linearexample_values.png)

### Rect Duplication
Duplication on X and Z

Width: # of dupes on X axis </br>
Length: # of dupes on Z axis

Offset variables:
| Var Name | Description |
| -------- | ----------- |
| W | Width of object |
| H | Height of object |

![Rect Preview](rectexample_preview.png) ![Rect Values](rectexample_values.png)
### Cube Duplication
Duplication on... you guessed it, X, Y and Z

Width: # of dupes on X axis </br>
Length: # of dupes on Z axis </br>
Height: # of dupes on Y axis

Offset variables:
| Var Name | Description |
| -------- | ----------- |
| W | Width of object |
| H | Height of object |

![Cube Preview](cubeexample_preview.png) ![Cube Values](cubeexample_values.png)

### Offsets

3 Vector3 inputs for position, rotation and scale. Each of these inputs has different variables that can be inserted into an expression to be evaluated for each dupe.

Examples:

3.404 * 2 : valid </br>
{varName} + 2 * (2 - 5) : valid </br>
34^{varName} - 4 : valid </br>

Note: Some of this code needs to be refactored after a couple of features were pushed out pretty fast