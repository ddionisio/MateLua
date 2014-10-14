## Scripting Symbols

NLua works by interfacing with either KopiLua or KeraLua based on which
preprocessor directive is defined:

`USE_KOPILUA` - KopiLua is a pure C# implementation of the Lua VM.

`USE_KERALUA` - KeraLua is an interop wrapper to the [NLua fork](https://github.com/NLua/lua)
of the original C VM (The DLL should be placed in a `Plugins\` folder in your project). 
This option is only available to **Unity Pro** users, since loading native DLLs is disabled
in the indie version.

If you know you are only ever going to use one option over the other, you can delete 
the unused folder (Assets/KopiLua/ or Assets/KeraLua/) to save a little space.

You also need to define `UNITY_3D` to suppress warnings about CLSC Attributes as
well as `LUA_CORE` & `CATCH_EXCEPTIONS` which are required by the VM.

Your **Scripting Define Symbols** list should end up looking something like:

```
UNITY_3D; USE_KOPILUA; LUA_CORE; CATCH_EXCEPTIONS
```

You may also notice other symbols used throughout NLua, none of these have yet
been tested for compatibility.