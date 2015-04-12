# Modlr

Model/Texture Editor for Minecraft.

## Features

W to move forward (zoom in)
S to move back (zoom out)
A to orbit left
D to orbit right
Scroll to change brush size 

### Sculpting

Right click to add blocks.
Left click to remove blocks.
Models can be loaded through a sidebar

### Painting

Right click to paint.
Left click to pick colour.
Colours and textures can be selected through a sidebar

## Future

Planned or suggested features

### Material-based editing

A selection of materials from Minecraft is provided for sculpting and painting.

This allows custom blocks to retain the feel of any resource pack.

When painting, a toggle determines whether you are creating a new material block
or painting the original texture (this invalidates the feel-retention).

### Magic brush

The magic brush adds or removes entire layers based on the selected surface.

The area of effect is adjusted with the mouse scroll wheel, or +/-.

### Rotation

22.5° increments.

### Multiple sub-blocks

Ties in with material-based editing

### Modification history

Undo and Redo.

### Variable grid scale

Grid scales of powers of two.

### Wavefront .obj import/export

### Face exclusion

Exclude faces from the generated model. Additionally makes planes possible.

### Advanced mode

UV offset and scaling.

### Scene selection

Various scenes as they would look with the current resource pack.

### Multiple loaded models

### Resource pack support

Locate "~/.minecraft" to load all installed resource packs, including official.

The current resource project is still saved separately to avoid overwriting, but
will probably be saved in the "resourcepacks" folder for easy testing.

### Symmetries

Mirror and/or rotate modification applications. Ties in with face texture
linking.

### Face texture rotation

## Changelog

### 0.1.0

Initial release.

## License

[Creative Commons - Attribution 4.0 International - CC-BY 4.0][cc-by-4.0]

## Credits

Kwibble, Elyon

[cc-by-4.0]: http://creativecommons.org/licenses/by/4.0/
