
# Character Clone 2D - Animations Replacer - Sprite Replacer

### 1. Clone Character 2D
Clone your entire character including Prefab, Animation, Animator, and Sprites, creating new references for each component.

#### Usage
- Create a folder with the name of your character. Inside this folder, place the character's prefab, an "animations" folder, and a "sprites" folder. Put the Animator Controller and the character's animations inside the "animations" folder, and place the character's sprites inside the "sprites" folder.
- IMPORTANT: Rename the model within the prefab to "Model". ![Model Rename Instruction](https://i.imgur.com/tzj935W.png)
- Right-click on the folder and select "Clone Character".
- A cloned directory with all dependencies and replacements will be created.

[Watch the Clone Character Tool in Action](https://mega.nz/file/Y4sBlQAQ#oBwyyxMbRoGp3s34_Ws5hkuy_aaM6LMn0_qioT8KcDI)

### 2. Replace Animations
Extend functionality for both `AnimatorOverrideController` and `AnimatorController` by replacing existing animations with others in the same folder and with matching names.

#### Usage
- Select either an `AnimatorOverrideController` or an `AnimatorController`.
- Right-click and select "Replace Animations".
- Animations will be replaced with those matching by name in the same folder.

[Watch the Animation Replacement Tool in Action](https://mega.nz/file/VwV2yBwb#r2aX7nvPi_MU6-q-8DVBJi0mkduU-k1wVZdG_NuT9Sw)

### 3. Sprite Replacer
Designed to replace sprites in animation clips, this tool is ideal for quick sprite changes across multiple animations.

#### Usage
- Open the tool from "Tools > Sprite Replacer" in the menu bar.
- Select the animations and sprites folders.
- Click "Replace Sprites" to execute replacements.

[Watch the Sprite Replacer Tool in Action](https://mega.nz/file/RxFQQA7R#RnVKi1p6U3-r5B1LRwnc0_gCn1AG26TJdGWsrhxVT_A)

## Installation

1. Clone or download this repository.
2. Copy the scripts into an `Editor` folder in your Unity project.

## Contributions

Contributions are welcome! If you have ideas for enhancing these tools or adding new features, feel free to create a pull request or open an issue.

_by majaus_
