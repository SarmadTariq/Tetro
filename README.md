# Tetro
Unity game inspired by Tetris

The game uses a custom shader to change vertex positions to
create the illusion that the objects are curved around the origin.
Furthermore, the shader also makes use of a depth buffer and
surface normals to create shadows in order to let players know
where the block is going to land.

As for the game itself, it uses the sticky line clear system which
is more sophisticated than the naive system.
It also deviates from tetris in that the position loops back if you
keep going in one direction.
