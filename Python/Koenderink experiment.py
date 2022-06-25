from tkinter import *
import random
import time
import colorsys
from datetime import datetime
import tkinter
import neptune.new as neptune
from tkinter import messagebox
import numpy as np
import csv
from tkinter import filedialog
from PIL import ImageColor
import matplotlib.pyplot as plt

run = neptune.init(
    project="emilim/afterimage",
    api_token="eyJhcGlfYWRkcmVzcyI6Imh0dHBzOi8vYXBwLm5lcHR1bmUuYWkiLCJhcGlfdXJsIjoiaHR0cHM6Ly9hcHAubmVwdHVuZS5haSIsImFwaV9rZXkiOiI5MGQxOGVhNi00N2JmLTQzOTctODM4Ni0xNDc1NmU0ZTA5MDkifQ==",
)  # your credentials riccardo  


def rgb(r, g, b):
    rgb  = tuple(map(int, (r, g, b)))
    return "#%02x%02x%02x" % rgb
def crgb(rgb):
    rgb  = tuple(map(int, rgb))
    return "#%02x%02x%02x" % rgb
def hsv2rgb(h,s,v):
    return tuple(round(i * 255) for i in colorsys.hsv_to_rgb(h,s,v))
def hilo(a, b, c):
    if c < b: b, c = c, b
    if b < a: a, b = b, a
    if c < b: b, c = c, b
    return a + c
def complement(rgb):
    r, g, b = rgb[0], rgb[1], rgb[2]
    k = hilo(r, g, b)
    return tuple(k - u for u in (r, g, b))
def newColor():
    global h, s, v, r, g, b, colorPredictedAfterImage, hPredicted, sPredicted, vPredicted
    h, s = random.uniform(0, 1), 0.9
    r, g, b = tuple(map(int, tuple([255*x for x in colorsys.hsv_to_rgb(h, s, v)])))
    colorPredictedAfterImage = tuple(map(int, complement((r, g, b))))
    hPredicted, sPredicted, vPredicted = colorsys.rgb_to_hsv(colorPredictedAfterImage[0], colorPredictedAfterImage[1], colorPredictedAfterImage[2])  
    
width, height = 1550, 900
v = 0.8
colorPredictedAfterImage = (0, 0, 0)
newColor()

radius = 175
x, y, x2, y2 = width/3 - radius, height/2 - radius, width/3 + radius , height/2 + radius

inducers, predictions, experiments, hInducers, hPredictions, hExperiments = [], [], [], [], [], []

root = Tk()
canvas = Canvas(root, width = width, height = height, bg=rgb(128, 128, 128))

time1 = ''
clock = Label(root, font=('times', 20, 'bold'), bg='green')
clock.pack(fill=BOTH, expand=1)
afterImagePredicted = ''


FMT = '%H:%M:%S'
currentTime = time.strftime(FMT)

def tick():
    global time1
    #time2 = str(int(time.strftime('%S')) - int(currentTime))
    time2 = (datetime.strptime(time.strftime(FMT), FMT) - datetime.strptime(currentTime, FMT)).total_seconds()
    if time2 != time1:
        time1 = time2
        clock.config(text=time2)
    clock.after(200, tick)
    if int(time2) == 10:
        drawAfterImage()

def drawFirstCircle(event=None):
    canvas.delete(ALL)# clear canvas first
    canvas.configure(bg=rgb(128, 128, 128))
    canvas.create_line(width/2-15, height/2, width/2+15, height/2, width=7, fill='black')
    canvas.create_line(width/2, height/2-15, width/2, height/2+15, width=7, fill='black')
    canvas.create_oval(x, y, x2, y2, fill=rgb(int(r*255), int(g*255), int(b*255)))
    resetTimer()

def drawAfterImage(event=None):
    global afterImagePredicted
    canvas.delete(ALL)
    canvas.create_line(width/2-15, height/2, width/2+15, height/2, width=7, fill='black')
    canvas.create_line(width/2, height/2-15, width/2, height/2+15, width=7, fill='black')
    canvas.create_oval(x, y, x2, y2, fill=rgb(160, 160, 160))
    afterImagePredicted = canvas.create_oval(x + width/3, y, x2 + width/3, y2, fill=crgb(colorPredictedAfterImage))

def redraw():
    global afterImagePredicted
    canvas.delete(afterImagePredicted)
    afterImagePredicted = canvas.create_oval(x + width/3, y, x2 + width/3, y2, fill=crgb(hsv2rgb(hPredicted, sPredicted, vPredicted)))

def hsvValues(event=None):
    global hPredicted, sPredicted, time1
    print(hPredicted, sPredicted)
    if time1 > 10:
        if event.keysym == 'Left' and hPredicted > 0.02:
            hPredicted -= 0.01
            redraw()
        elif event.keysym == 'Right' and hPredicted < 0.98:
            hPredicted += 0.01
            redraw()
        if event.keysym == 'Up' and sPredicted < 0.90:
            sPredicted += 0.02
            redraw()
        elif event.keysym == 'Down' and sPredicted > 0.02:
            sPredicted -= 0.02
            redraw()

def saveImage(event=None):
    inducers.append(rgb(r, g, b)) # inducer
    predictions.append(crgb(colorPredictedAfterImage)) # predicted after image color
    experiments.append(crgb(colorsys.hsv_to_rgb(hPredicted, sPredicted, vPredicted))) # actual after image color seen by the subject

    hInducers.append(colorsys.rgb_to_hsv(r, g, b)[0] * 360) # inducer hue
    hPredictions.append(colorsys.rgb_to_hsv(colorPredictedAfterImage[0], colorPredictedAfterImage[1], colorPredictedAfterImage[2])[0] * 360) # predicted after image hue
    hExperiments.append(hPredicted * 360) # actual after image hue seen by the subject

def grid():
    canvas.delete(ALL)# clear canvas first
    w = canvas.winfo_width() # Get current width of canvas
    h = canvas.winfo_height() # Get current height of canvas
    
    for i in range(0, w, 20):
        for j in range(0, h, 20):
            canvas.create_rectangle(i, j, i+w/20, j+h/20, fill=rgb(random.uniform(0, 255), random.uniform(0, 255), random.uniform(0, 255)), width=5)

def newCircle(event=None):
    saveImage()
    newColor()
    grid()
    # delay 1 sec
    root.after(1000, drawFirstCircle)
    

def resetTimer(event=None):
    global time1, currentTime
    time1 = ''
    currentTime = time.strftime(FMT)

drawFirstCircle()
def main(width, height):
    print(width, height)


#btn = Button(root, text = 'Click me !', bd = '5', command=main(canvas.winfo_width(), canvas.winfo_height()))
#btn.pack(side = 'bottom')   
canvas.pack(fill=tkinter.BOTH, side=tkinter.LEFT, expand=True)

root.bind("<space>", drawFirstCircle)
root.bind("<Left>", hsvValues)
root.bind("<Right>", hsvValues)
root.bind("<Up>", hsvValues)
root.bind("<Down>", hsvValues)
root.bind("<Return>", newCircle)

tick()

fig, axs = plt.subplots(2)

def on_closing():
    if messagebox.askokcancel("End of experiment", "Shall we save the results?"):
        root.destroy()
        params = {"inducer": inducers, "prediction": predictions, "experimental": experiments, "hInducers": hInducers}
        run["parameters"] = params
        run.stop()

        saved = np.array([inducers, predictions, experiments, hInducers, hPredictions, hExperiments])
        
        directory = filedialog.asksaveasfilename()
        
        with open(directory+'.csv', 'w', newline='') as file:   
            mywriter = csv.writer(file, delimiter=',')
            mywriter.writerows(zip(*saved))
        
        for i in range(len(inducers)):
            #print(inducers[i], predictions[i], experiments[i])
            r, g, b = ImageColor.getcolor(inducers[i], "RGB")
            hInducer, s, v = colorsys.rgb_to_hsv(r/255, g/255, b/255)

            r, g, b = ImageColor.getcolor(predictions[i], "RGB")
            hPrediction, s, v = colorsys.rgb_to_hsv(r/255, g/255, b/255)

            r, g, b = ImageColor.getcolor(experiments[i], "RGB")
            hExperiment, s, v = colorsys.rgb_to_hsv(r/255, g/255, b/255)

            print(hInducer * 360, hPrediction * 360, hExperiment * 360)

            axs[0].scatter(hInducer * 360, hPrediction * 360, color=inducers[i])
            axs[1].scatter(hInducer * 360, hExperiment * 360, color=inducers[i])
        plt.show()


root.protocol("WM_DELETE_WINDOW", on_closing)
root.mainloop()

#commento due