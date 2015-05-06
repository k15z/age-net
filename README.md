# age-net

Train a neural network to look at photographs and make a reasonable guess at the age of the subject. The training data set was collected from the IMDB website and contains over 6000 photos sorted by the age.

## data

```
age-net
|-img
| |-16
|  |-0a98a69aef9545d28d5743ee7f776f05.png
|  |-1ad2a212a1b7419bbb363163499a7099.png
|   ...
|  |-efcada2b3f2a4486a6c46fa5d5ff3e15.png
| |-17
| |-18
|  ...
| |-85
```

## design
This project uses a fairly standard feedforward neural network based on the multilayer perceptron. It is set up to support a configurable number of layers and a configurable number of nodes in each layer.

```
3996 input nodes
999 hidden nodes
333 hidden nodes
100 output nodes
```

### inputs
The inputs are simply the brightness values of each individual pixel in the 54 by 74 images, scaled to a value between 0.0f and 1.0f. No more image preprocessing is performed here... mostly because I don't know how to do it.

### outputs
The outputs are encoded by creating a floating point number of size 100, initialized to all 0s. If the expected age was 18, then the 18th values would be set to 1.0f, the 17th and 19th values would be set to 0.3f, and the 16th and 20th values would be set to 0.1f.
