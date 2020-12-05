Updatable Radial slider settings:
Clockwise direction
WholeNumbers - slider takes only whole number values
Update speed - affects the speed for slider handler moving to target value 
Raycast limits - allows you to ignore some clicks in the center and outside the slider circle
Border Crossing Resistance - stops the slider when crossing over the 0-1 border. Set it value to 0 for smooth slider moving across start-end border.
AngleLimit - allows you to setting max slider angle, in demo scene presented sliders with 360, 180 and 90 angle limits.

Gradient customization implemented by GradientTextureGenerator script, applyed to slider "fill" image. It generates gradient texture at Start and applies it to custom optimized shader. 
You can change gradient direction by changing Angle value via GradientTextureGenerator.
