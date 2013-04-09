using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public class GameplayObject
    {
        private Texture2D _texture;
        private ObjectStatus _status;

        private Color _color = Color.White;
        private Vector2 _position = Vector2.Zero;
        private TimeSpan _dieTime = TimeSpan.Zero;
        private float _diePercent = 0.0f;

        public GameplayObject()
        {
            Rotation = 0.0f;
            Alpha = 1.0f;
            Initialize();
        }

        public Body Body { get; protected set; }

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                if (value != null)
                {
                    _texture = value;
                }
            }
        }

        public virtual Vector2 Origin
        {
            get { return new Vector2(_texture.Width / 2f, _texture.Height / 2f); }
        }

        public float Alpha { get; set; }

        public Color Color
        {
            get { return _color * Alpha; }
            set { _color = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Rotation { get; set; }

        public TimeSpan DieTime
        {
            get { return _dieTime; }
            set { _dieTime = value; }
        }

        public ObjectStatus Status
        {
            get { return _status; }
        }

        protected float DiePercent
        {
            get { return _diePercent; }
        }

        public virtual void Die()
        {
            if (_status == ObjectStatus.Active)
            {
                _status = _dieTime != TimeSpan.Zero ? ObjectStatus.Dying : ObjectStatus.Dead;
            }
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        public virtual void Initialize()
        {
            _status = ObjectStatus.Active;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_status == ObjectStatus.Active)
            {
                // TODO: Update location
                if (Body != null)
                {
                    _position = ConvertUnits.ToDisplayUnits(Body.Position);
                    //Rotation = Body.Rotation;
                }
            }
            else if (_status == ObjectStatus.Dying)
            {
                Dying(gameTime);

                if (Body != null)
                {
                    _position = ConvertUnits.ToDisplayUnits(Body.Position);
                    //Rotation = Body.Rotation;
                }
            }
            else if (_status == ObjectStatus.Dead)
            {
                Dead(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (spriteBatch != null & _texture != null)
            {
                spriteBatch.Draw(_texture, _position, null, Color, Rotation, Origin, 1.0f, SpriteEffects.None, 0.0f);
            }
        }

        public virtual void Dying(GameTime gameTime)
        {
            if (_diePercent >= 1f)
            {
                _status = ObjectStatus.Dead;
            }
            else
            {
                var dieDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / _dieTime.TotalMilliseconds);
                _diePercent += dieDelta;
            }
        }

        public virtual void Dead(GameTime gameTime) { }
    }
}