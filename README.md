# ThoughtBubble
A 3D virtual canvas powered by Oculus Rift and Myo Armband

Thought Bubble started as a 2D drawing tool and has transformed into a 3D virtual canvas which users can fill with their thoughts and ideas.

Thought Bubble uses the Myo Armband to capture the motion of the user's arm as they move through space in any direction. The Oculus Rift allows for a truly 3D experience by making the user the pivot point of their own bubble.

Users clench their first to draw on the canvas and unclench to stop drawing. "Wave out" (swiping your hand right) brings up the next canvas and "Wave in" (swiping your hand left) brings up the previous canvas. Bubbles are saved on exit, so users will never lose their data. Users can double tap on any of the buttons, to change the drawing color.

Having never worked with Myo, Oculus, Unity, or C#, the team had quite a few struggles along the way. Integration between Oculus and Myo was difficult because we had to make sure that the two were synced in a way that made it look very natural. We also spent quite some time trying to understand game object generation in unity to understand how to change the color of objects after creation. (You have to change the material on the object, not the object itself)