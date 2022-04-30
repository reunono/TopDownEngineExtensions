Requires A* Pathfinding Project to work.

Unity has no native 2d pathfinding solution, and this is the best option afaict

To install, just copy into your source dir.

Make sure to set the composite colliders on any obstacle tilemaps (i.e. walls) to be "polygons" instead of "outlines"
Also, make sure you put a `Seeker.cs` from A* Pathfinding onto whatever AI Character object uses these components
Lastly, make sure you have a global object with the  `Pathfinding` component to define the grid your AIs will search on.

See A* Pathfinding Project's docs for more details on how this works.

This code is hasty, made for a game jam, provided-as-is in case it helps someone but I will not be maintaining or supporting this integration