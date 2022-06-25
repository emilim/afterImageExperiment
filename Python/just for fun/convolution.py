import matplotlib.pyplot as plt
import numpy as np

interval = 10

energyFunction = []
filter = []
absorbed = []
linspace = []

def f(x):
    if -interval < x < interval:
        return np.sin(x)**2 + np.cos(x)**2
    else:
        return 0

def g(x):
  if -interval < x < interval:
    return 10
  else:
    return 0

def convo(x):
    integral = 0
    xt = -interval
    dx = 0.01
    while xt < interval:
        integral  += f(xt) * g(x - xt) * dx
        xt = xt + dx
    return integral

t = -interval - 2
dt = 0.05
while t < interval + 2:
    linspace.append(t)
    energyFunction.append(f(t))
    filter.append(g(t))
    absorbed.append(convo(t))
    #plt.scatter(t, f(t), c='r')
    #plt.scatter(t, g(t), c='g')
    #plt.scatter(t, convo(t), c='b')
    t = t + dt

plt.plot(linspace, energyFunction, c='r')
plt.plot(linspace, filter, c='g')
plt.plot(linspace, absorbed, c='b')
plt.show()