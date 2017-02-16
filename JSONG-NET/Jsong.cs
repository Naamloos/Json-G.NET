using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace JsonG
{
    public class JImage
    {
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version;

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment;

        [JsonProperty("transparancy", NullValueHandling = NullValueHandling.Ignore)]
        public bool Transparancy;

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public JImageSize Size;

        [JsonProperty("layers", NullValueHandling = NullValueHandling.Ignore)]
        public List<JImageLayer> Layers;

        public Bitmap ToBitmap()
        {
            JImage j = this;
            Bitmap b = new Bitmap(j.Size.Width, j.Size.Height);
            foreach (JImageLayer layer in j.Layers)
            {
                int width = b.Size.Width;
                int height = b.Size.Height;
                int hi = 0;
                while (hi < height)
                {
                    int wi = 0;
                    while (wi < width)
                    {
                        JImagePixel jip = layer.Pixels.Find(x => x.Position.x == wi && x.Position.y == hi);
                        Color c = new Color();
                        if (jip != null)
                        {
                            c = Color.FromArgb(jip.Color.Alpha, jip.Color.Red, jip.Color.Green, jip.Color.Blue);
                        }else
                        {
                            c = Color.FromArgb(layer.DefaultColor.Alpha, layer.DefaultColor.Red, layer.DefaultColor.Green, layer.DefaultColor.Blue);
                        }
                        b.SetPixel(wi, hi, c);
                        wi++;
                    }
                    hi++;
                }
            }
            return b;
        }

        public string GetJson()
        {
            return JObject.FromObject(this).ToString();
        }

        public static JImage FromJson(string s)
        {
            return JObject.Parse(s).ToObject<JImage>();
        }

        public static JImage FromBitmap(Bitmap g, string Comment = "")
        {
            JImage j = new JImage()
            {
                Size = new JImageSize()
                {
                    Height = g.Height,
                    Width = g.Width
                },
                // not sure on how to
                Transparancy = true,
                Version = "1.0",
                Comment = Comment
            };
            JImageLayer l = new JImageLayer()
            {
                DefaultColor = new JImageColor()
                {
                    Blue = 0,
                    Alpha = 0,
                    Green = 0,
                    Red = 0
                },
            };

            List<JImagePixel> p = new List<JImagePixel>();

            int width = g.Width;
            int height = g.Height;
            int hi = 0;
            while (hi < height)
            {
                int wi = 0;
                while (wi < width)
                {
                    Color c = g.GetPixel(wi, hi);
                    p.Add(new JImagePixel()
                    {
                        Position = new JImagePixelPosition()
                        {
                            x = wi,
                            y = hi
                        },
                        Color = new JImageColor()
                        {
                            Alpha = c.A,
                            Blue = c.B,
                            Green = c.G,
                            Red = c.R
                        }
                    });
                    wi++;
                }
                hi++;
            }
            l.Pixels = p;
            j.Layers = new List<JImageLayer>() { l };
            return j;
        }
    }

    public class JImageSize
    {
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int Width;
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int Height;
    }

    public class JImageLayer
    {
        [JsonProperty("default_color", NullValueHandling = NullValueHandling.Ignore)]
        public JImageColor DefaultColor;
        [JsonProperty("pixels", NullValueHandling = NullValueHandling.Ignore)]
        public List<JImagePixel> Pixels;
    }

    public class JImagePixel
    {
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public JImagePixelPosition Position;

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public JImageColor Color;

        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment;
    }

    public class JImagePixelPosition
    {
        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public int x;
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public int y;
    }

    public class JImageColor
    {
        [JsonProperty("red", NullValueHandling = NullValueHandling.Ignore)]
        public int Red;

        [JsonProperty("blue", NullValueHandling = NullValueHandling.Ignore)]
        public int Blue;

        [JsonProperty("green", NullValueHandling = NullValueHandling.Ignore)]
        public int Green;

        [JsonProperty("alpha", NullValueHandling = NullValueHandling.Ignore)]
        public int Alpha;
    }
}
