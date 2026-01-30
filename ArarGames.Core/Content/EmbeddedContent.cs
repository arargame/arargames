using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Reflection;

namespace ArarGames.Core.Content
{
    public static class EmbeddedContent
    {
        /// <summary>
        /// Loads a Texture2D from the assembly's embedded resources.
        /// Ensure the file is marked as 'Embedded Resource' in the project properties.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice used to create the texture.</param>
        /// <param name="resourceName">The name of the resource (e.g., "ArarGames.Core.Resources.ArarGamesLogo.png").</param>
        /// <returns>The loaded Texture2D.</returns>
        public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found. Make sure the Build Action is set to 'Embedded Resource'.");

                return Texture2D.FromStream(graphicsDevice, stream);
            }
        }

        /// <summary>
        /// Loads the default ArarGames Logo.
        /// </summary>
        public static Texture2D LoadDefaultLogo(GraphicsDevice graphicsDevice)
        {
            return LoadTexture(graphicsDevice, "ArarGames.Core.Resources.ArarGamesLogo.png");
        }
    }
}
