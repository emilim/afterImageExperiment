import csv
from PIL import ImageColor
import matplotlib.pyplot as plt
import numpy as np
import colorsys

inducers = []
predictions = []
experiments = []

with open('.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')
    line_count = 0
    for row in csv_reader:
        inducer = ImageColor.getcolor(row[0], "RGB")
        prediction = ImageColor.getcolor(row[1], "RGB")
        experiment = ImageColor.getcolor(row[2], "RGB")

        inducers.append(row[0])
        predictions.append(row[1])
        experiments.append(row[2])

        print(f'\t{inducer} is the inducer, {prediction} is the predicted color, and {experiment} is the experimental result.')
        line_count += 1
    print(f'Processed {line_count} different afterimages.')

#visualize inducers and predictions
fig, axs = plt.subplots(2)
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
