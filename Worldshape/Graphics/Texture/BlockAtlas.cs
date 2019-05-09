using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Worldshape.Configuration;
using Worldshape.Layout;
using Worldshape.Logging;

namespace Worldshape.Graphics.Texture
{
    public class BlockAtlas
    {
        private readonly Dictionary<string, BlockRenderData> _atlas = new Dictionary<string, BlockRenderData>();
        public BlockRenderData this[string name] => _atlas.TryGetValue(name, out var value) ? value : null;
        public int Texture { get; }

        public BlockAtlas(MappingEngine mappings, int textureResolution)
        {
			Lumberjack.Debug("Generating texture atlas");
            var pointers = new List<TexturePointer>();
            var resolution = new Size(textureResolution, textureResolution);
            foreach (var set in mappings.Mappings)
            {
	            if (set.Texture.Count == 0)
		            continue;
	            foreach (var texture in set.Texture)
	            {
		            if (texture.Contains(","))
		            {
			            var subTextures = texture.Split(',');
			            pointers.AddRange(subTextures.Select((subTexture, i) => new TexturePointer($"{set.Name}${i}", Path.Combine(set.TextureDir, subTexture), resolution)));
		            }
		            else
					{
						pointers.Add(new TexturePointer(set.Name, Path.Combine(set.TextureDir, texture),
							resolution));
					}
	            }
            }

            var size = 1024;
            var packed = Pack(size, size, pointers);
            if (!packed)
                throw new ArgumentException("Failed to create texture atlas, atlas too small!");

            var srcTexRect = new Rectangle(0, 0, textureResolution, textureResolution);

            var bmpAtlas = new Bitmap(size, size);
            using (var gfx = System.Drawing.Graphics.FromImage(bmpAtlas))
            {
                foreach (var pointer in pointers)
                {
                    using (var bmpTexture = Image.FromFile(pointer.TexturePath))
                        gfx.DrawImage(bmpTexture, new Rectangle(pointer.Position, pointer.Size), srcTexRect,
                            GraphicsUnit.Pixel);

                    var minU = pointer.Position.X / (float)size;
                    var minV = pointer.Position.Y / (float)size;
                    var maxU = (pointer.Position.X + pointer.Size.Width) / (float)size;
                    var maxV = (pointer.Position.Y + pointer.Size.Height) / (float)size;

                    var blockname = pointer.TextureName.Split('$')[0];

					if (!_atlas.ContainsKey(blockname))
                        _atlas.Add(blockname, new BlockRenderData(mappings[blockname]));

                    _atlas[blockname].Textures.Add(new TexCoord(minU, minV, maxU, maxV));
                }

                if (Program.Config.SaveAtlas)
                {
	                bmpAtlas.Save("debugatlas.png");
					Lumberjack.Info("Saved atlas as debugatlas.png");
                }

                Texture = CreateTexture(bmpAtlas);
			}
			Lumberjack.Debug($"Packed {pointers.Count} x{textureResolution} textures in a {size}x atlas");
		}

        private static int CreateTexture(Bitmap bitmap)
        {
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out int tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

            return tex;
        }

        private static int ToNextPow2(int x)
        {
            if (x < 0) { return 0; }
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        private static bool Pack(int width, int height, IEnumerable<TexturePointer> children)
        {
            var startNode = new Node
            {
                Rectangle = new Rectangle(0, 0, width, height)
            };

            foreach (var entry in children)
            {
                var rect = new Rectangle(0, 0, entry.Size.Width, entry.Size.Height);

                var node = startNode.Insert(rect);

                if (node != null)
                    entry.Position = node.Rectangle.Location;
                else
                    return false;
            }

            return true;
        }
    }
}
