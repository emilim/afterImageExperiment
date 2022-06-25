import colorsys
import matplotlib.pyplot as plt
import numpy as np

def hilo(a, b, c):
    if c < b: b, c = c, b
    if b < a: a, b = b, a
    if c < b: b, c = c, b
    return a + c
def complement(rgb):
    r, g, b = rgb[0], rgb[1], rgb[2]
    k = hilo(r, g, b)
    return tuple(k - u for u in (r, g, b))

hs = []
hsNew = []
hsv = []
hsvComplementary = []
hsvCNew = []

for h in range(360):
    hs.append(h)
    rgb = colorsys.hsv_to_rgb(hs[h]/360, hs[h]/255, hs[h]/255)
    hsv.append(colorsys.rgb_to_hsv(rgb[0], rgb[1], rgb[2]))
    complementaryRGB = complement(rgb)
    hsvComplementary.append(colorsys.rgb_to_hsv(complementaryRGB[0], complementaryRGB[1], complementaryRGB[2]))

for i in range(len(hs)):
    hsNew.append(hsv[i][0])
    hsvCNew.append(hsvComplementary[i][0])

plt.plot(hs, hsNew)
plt.scatter(hs, hsvCNew, c='r')
plt.show()