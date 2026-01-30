# ArarGames Framework

A robust MonoGame-based framework for 2D (and future 3D) game development.
Designed to standardize the workflow for ArarGames projects.

## Architecture

### Core
- **GameObject**: Base class for all entities with hierarchical support (Parent/Children).
- **Sprite**: 2D entity with Texture, Color, Rotation, Scale.
- **MeshObject**: Placeholder for 3D entity support.

### Screens
- **ScreenManager**: Stack-based screen management (Push/Pop).
- **Screen**: Abstract base class for game states (Menu, Gameplay, settings).

### Input
- **InputManager**: Unifies Keyboard and Mouse state polling.

### UI
- **Button**: Clickable sprite with Hover states.
- **Label**: Text rendering component.

### Audio
- **SoundManager**: Static wrapper for SoundEffects and Songs.

### Utils
- **MathUtil**: Helpers for Vectors and math.
- **Timer**: Simple event-based timer.

### Content & Resources
- **EmbeddedContent**: Helper to load resources embedded in the DLL.
  - `LoadDefaultLogo(GraphicsDevice)`: Returns the ArarGames logo.
  - `LoadTexture(GraphicsDevice, fullResourceName)`: Loads any embedded texture.
- **Resources**: Default embedded assets (e.g., Logo) reside in `ArarGames.Core/Resources`.


## Usage Example

### Creating a Screen
```csharp
public class MyGameScreen : Screen
{
    Sprite player;

    public override void LoadContent()
    {
        // ... Load textures ...
        player = new Sprite(texture);
        player.Position = new Vector2(100, 100);
    }

    public override void Update(GameTime gameTime)
    {
        player.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        player.Draw(spriteBatch);
    }
}
```

### Initializing
In your `Game1.cs`:
```csharp
ScreenManager screens = new ScreenManager(this);
Components.Add(screens);
InputManager.Instance.Update(); // Call in Update loop
```

### Loading Embedded Resources
```csharp
using ArarGames.Core.Content;

// Load default logo
Texture2D logo = EmbeddedContent.LoadDefaultLogo(GraphicsDevice);

// Load custom embedded resource
Texture2D myTexture = EmbeddedContent.LoadTexture(GraphicsDevice, "ArarGames.Core.Resources.MyFolder.MyImage.png");
```
