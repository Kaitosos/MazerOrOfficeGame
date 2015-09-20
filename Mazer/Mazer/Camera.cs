using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mazer
{
    public class Camera
    {
        public float Zoom = 1f;
        public float Speed = 1;

        private int windowWith;
        private int windowHight;

        public Camera(int windowWith, int windowHight)
        {
            this.windowHight = windowHight;
            this.windowWith = windowWith;
        }

        public void opt_Update(int windowWith, int windowHight)
        {
            this.windowHight = windowHight;
            this.windowWith = windowWith;
        }
        /// <summary>
        /// Position der Kamera.
        /// </summary>

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 position;

        public void Update(Vector2 playerPos, int playersize, int windowMoveRight, int windowMoveLeft, int windowUDBoarder)
        {
            int X;
            do
            {
                X = 0;
                // Kamera bewegen
                if (playerPos.X <= -1 * this.position.X + windowMoveRight)
                {//Nach Rechts
                    this.position.X += 1;
                    X++;
                }
                if (playerPos.X >= -1 * this.position.X + (this.windowWith / this.Zoom) - playersize - windowMoveLeft)
                {//Nach Links
                    this.position.X -= 1;
                    X++;
                }
                if (playerPos.Y <= -1 * this.position.Y + windowUDBoarder)
                {
                    this.position.Y += 1;
                    X++;
                }
                if (playerPos.Y >= -1 * this.position.Y + (this.windowHight / this.Zoom) - playersize - windowUDBoarder)
                {//Nach Links
                    this.position.Y -= 1;
                    X++;
                }
            } while (X != 0);
            this.position = new Vector2(this.position.X, this.position.Y);
        }

        public void ZoomOut()
        {
            this.Zoom -= 0.1f;
            if (this.Zoom <= 0f)
                this.Zoom = 0.1f;
        }

        public void ZoomNormal()
        {
            this.Zoom += 0.1f;
            if (this.Zoom >= 1.1f)
                this.Zoom = 1f;
        }
        public void SetNormal()
        {
            this.Zoom = 1f;
        }
        public Matrix GetMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(position, 0)) * Matrix.CreateScale(Zoom);
        }
    }
}

