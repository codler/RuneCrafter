using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RuneCrafter
{
    class Camera
    {
        public Matrix cameraView;
        public Matrix cameraProjection = Matrix.CreatePerspectiveFieldOfView(1, 1, 1, 1000);
        public Vector3 cameraPosition = new Vector3(0, 0, -100);
        public Vector3 cameraTarget = new Vector3(0, 0, 0);

        public float mHeight = 20f;
        public float mDistance = 0f;
        public float distanceOffset = -20f;

        public float height
        {
            get
            {
                return mHeight;
            }
            set
            {
                mHeight = MathHelper.Clamp( value, 0f, 89.9f);
            }
        }
        public float distance
        {
            get
            {
                return mDistance;
            }
            set
            {
                mDistance = MathHelper.Clamp(value, -10000f / GameConstants.scrollSensitive, 0f);
            }
        }

        public void init() 
        {
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }

        public void update(GameObject target) 
        {
            cameraTarget = target.position;

            cameraPosition = target.position + Vector3.Transform(new Vector3(0, 0, mDistance + distanceOffset ),  Matrix.CreateRotationX(mHeight / 60f) * Matrix.CreateRotationY(target.rotation));
            ;
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }
    }
}
