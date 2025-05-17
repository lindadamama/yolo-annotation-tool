# YoloAnnotationTool2

yolo-annotation-tool - it's a simple tool for annotating objects in YOLO format (Detect, OBB, Segment, Classify, Pose). 
It creates a project structure compatible with YOLOv8/v11, allows you to annotate photos, and saves annotation data in .txt in YOLO format. Also, after completion, you can archive the project folder to start training models immediately  

## Download

1. Go to the [Releases] section (https://github.com/bravo-maestr0/yolo-annotation-tool/releases)
2. Download the `.zip' archive of the latest version
3. Unzip it
4. Run `AnnotationTool.exe`.

## How to create a project

First, you need to set up a directory to store your projects: Right click on File -> "Set folder for saving projects"
![зображення](https://github.com/user-attachments/assets/1cae0500-a613-412e-9d40-a7694a42210a)

Next, to create a project, enter the name of the project, the type of annotations
After that, Right click on File -> "+ New Project".
The folder in a format "ProjectName_SelectedAnnotationType" will be created in the directory you set for saving projects

## How to use

### Prepare for annotation
Before adding any annotations, you need to add photos

![зображення](https://github.com/user-attachments/assets/549b2efa-c536-4bf5-8dfc-5da54fc8dc5f)

Then, you need to add classes

![зображення](https://github.com/user-attachments/assets/da28cdfe-455f-4d30-b874-dd10f1eea459)

![зображення](https://github.com/user-attachments/assets/13f3ab32-7359-4991-b1a6-e9ee039aeaa1)
#### Detect
1. Draw bounding boxes with your mouse
2. Click "Save" to save coordinates of painted boxes
#### OBB
1. Draw length and angle of bounding box with your mouse
2. Hold down "Shift" to extend width of bounding box
3. Click "Save" to save coordinates of painted boxes
#### Segment
1. Click with your mouse to draw keypoints
2. To finish drawing the segment, click near the start point of the segment
3. Click "Save" to save coordinates of painted segments
#### Pose
1. Follow the instructions under the canvas to draw pose
2. Click "Save" to save coordinates of painted poses
#### Classify
There is no drawing so it's just organisation of photos
1. Add class with the name of object that you want to classify (directory with the name of class will be created)
2. Add photos to the current selected class
Then, you can swap between classes and see which photo is in which class
#### ⚙ Shortcuts
CTRL + Z - remove last added figure
(Click save after removing unwanted figures)
