# Hızlı Başlangıç Rehberi (Getting Started)

Bu rehber, **ArarGames Framework** kullanarak sıfırdan yeni bir MonoGame projesi başlatma adımlarını, `Game1.cs` yapılandırmasını ve ilk oyun ekranını oluşturmayı adım adım açıklar.

---

## 1. Yeni Proje Oluşturma

.NET CLI kullanarak yeni bir MonoGame DesktopGL projesi oluşturabilirsiniz:

```bash
# MonoGame şablonları yüklü değilse yükleyin:
dotnet new install MonoGame.Templates.CSharp

# Yeni DesktopGL projesi oluşturun:
dotnet new mgdesktopgl -n MyGame.Desktop
```

---

## 2. Proje Referanslarının Eklenmesi

Oyun projenize `ArarGames.Core` ve `ArarGames.Engine` projelerini ekleyin:

### .csproj Yapılandırması:
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <RootNamespace>MyGame</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.4.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\ArarGames.Core\ArarGames.Core.csproj" />
    <ProjectReference Include="..\ArarGames.Engine\ArarGames.Engine.csproj" />
  </ItemGroup>

</Project>
```

---

## 3. İlk Oyun Ekranını Oluşturma

Tüm ekranlar `ArarGames.Engine.Screens.GameScreen` abstract sınıfından türer.

`Screens/MainMenuScreen.cs`:
```csharp
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArarGames.Engine.Input;
using ArarGames.Engine.Screens;

namespace MyGame.Screens;

/// <summary>
/// Represents the main menu screen of the game.
/// </summary>
public class MainMenuScreen : GameScreen
{
    private SpriteFont? _font;
    private string _titleText = "MY AWESOME GAME";

    public MainMenuScreen(GameContext context) : base(context)
    {
    }

    public override void LoadContent()
    {
        // Load fonts or textures via Context.Content
        // _font = Context.Content.Load<SpriteFont>("Fonts/MenuFont");
    }

    public override void HandleInput(in InputState input)
    {
        // Start game on Enter or Fire button
        if (input.ConfirmJustPressed || input.FireJustPressed)
        {
            // Transition to gameplay
            // Context.Screens.Replace(new GameplayScreen(Context));
        }

        // Exit or Back
        if (input.BackJustPressed)
        {
            // Handled or exit
        }
    }

    public override void Update(GameTime gameTime, bool isCovered)
    {
        if (isCovered) return;

        // Menu animations and logic here
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Drawing occurs within Virtual Resolution coordinates (e.g. 1280x720)
        // Draw background, buttons, title
    }
}
```

---

## 4. Game1.cs Kurulumu

`Game1.cs` sınıfı, `ScreenManager` ve `GameContext` nesnelerini başlatır, sanal çözünürlüğü ayarlar ve girdi durumunu her karede toplar.

`Game1.cs`:
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArarGames.Engine.Audio;
using ArarGames.Engine.Input;
using ArarGames.Engine.Screens;
using MyGame.Screens;

namespace MyGame;

/// <summary>
/// The main entry game class initializing ArarGames Framework systems.
/// </summary>
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    
    private ScreenManager _screenManager = null!;
    private GameContext _context = null!;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            IsFullScreen = false,
            SynchronizeWithVerticalRetrace = true
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // 1. Initialize ScreenManager with target virtual resolution
        _screenManager = new ScreenManager
        {
            VirtualWidth = 1280,
            VirtualHeight = 720
        };

        // 2. Build GameContext (Central service locator for screens)
        _context = new GameContext(_screenManager, GraphicsDevice, Content)
        {
            // Optional: configure audio service
            Audio = new SoundManager(name => Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(name))
        };

        // 3. Initialize ScreenManager and push the first screen
        _screenManager.Initialize();
        _screenManager.Push(new MainMenuScreen(_context));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        // 1. Gather hardware input and map to unified InputState
        var currentKeyboard = Keyboard.GetState();
        var currentMouse = Mouse.GetState();

        // Convert mouse physical position to virtual resolution
        Vector2 mouseScreenPos = new Vector2(currentMouse.X, currentMouse.Y);
        Vector2 mouseVirtualPos = _screenManager.ScreenToVirtual(mouseScreenPos);

        // Calculate move direction from WASD / Arrow keys
        Vector2 move = Vector2.Zero;
        if (currentKeyboard.IsKeyDown(Keys.W) || currentKeyboard.IsKeyDown(Keys.Up)) move.Y -= 1;
        if (currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Down)) move.Y += 1;
        if (currentKeyboard.IsKeyDown(Keys.A) || currentKeyboard.IsKeyDown(Keys.Left)) move.X -= 1;
        if (currentKeyboard.IsKeyDown(Keys.D) || currentKeyboard.IsKeyDown(Keys.Right)) move.X += 1;
        if (move != Vector2.Zero) move.Normalize();

        var input = new InputState
        {
            MoveDirection = move,
            CursorPosition = mouseVirtualPos,
            FirePressed = currentMouse.LeftButton == ButtonState.Pressed || currentKeyboard.IsKeyDown(Keys.Space),
            FireJustPressed = (currentMouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
                              || (currentKeyboard.IsKeyDown(Keys.Space) && _prevKeyboard.IsKeyUp(Keys.Space)),
            ConfirmJustPressed = currentKeyboard.IsKeyDown(Keys.Enter) && _prevKeyboard.IsKeyUp(Keys.Enter),
            BackJustPressed = currentKeyboard.IsKeyDown(Keys.Escape) && _prevKeyboard.IsKeyUp(Keys.Escape),
            PauseJustPressed = currentKeyboard.IsKeyDown(Keys.P) && _prevKeyboard.IsKeyUp(Keys.P)
        };

        _prevKeyboard = currentKeyboard;
        _prevMouse = currentMouse;

        // 2. Global audio update
        _context.Audio?.Update(gameTime);

        // 3. Update active screens
        _screenManager.Update(gameTime, in input);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Calculate letterbox/pillarbox viewport and matrix
        _screenManager.CalculateMatrix(GraphicsDevice);
        GraphicsDevice.Clear(Color.Black);

        // Begin drawing with global scale matrix
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            _screenManager.GlobalScaleMatrix);

        _screenManager.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
```

---

## 5. Program.cs Giriş Noktası

`Program.cs`:
```csharp
using var game = new MyGame.Game1();
game.Run();
```

Tebrikler! İlk ArarGames projeniz hazır. Artık yeni ekranlar, sesler, kayıt mekanizmaları ve olaylar eklemeye başlayabilirsiniz.
