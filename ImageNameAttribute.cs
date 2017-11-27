using System;

namespace WrightCover
{
    internal class ImageNameAttribute : Attribute
    {
        public string ImageName;

        public ImageNameAttribute(string imageName)
        {
            this.ImageName = imageName;
        }
    }
}
