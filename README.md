Unity UI Trend Graph
====================
This implements a UI trend graph display for single variable timeseries.

Getting Started
---------------
Create a Canvas and add the TrendGraph Prefab to your project. The TrendGraph
is dynamically resizable so you can have it stretch or anchor with a fixed width
and height. You can also adjust the scale on the TrendGraph and all the elements
should scale correctly.

To add timeseries data records get a reference to the TrendGraphController, then
just call the add method and pass it a DateTime and float value. The rendering
should take care of itself.

If you need to procedurally set the public yMax, yMin, Timebase, and Value 
fields call OnValidate to set the text.

Caveats
-------
Need Unity 4.6+ for the UI. If not using Unity 5, need a pro license to use the
GL class. The GL class isn't available on all the mobile platforms, maybe 
others as well?.
