using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace RuneCrafter
{
    class GameObject
    {
        public AnimationController animationController;
        public SkinnedModel model;
        public int activeAnimationClipIndex = 0;
        public Vector3 position = new Vector3(0f,0f,0f);
        public Vector3 prevPosition = new Vector3(0f, 0f, 0f);
        public Vector3 assignPosition = new Vector3(0f, 0f, 0f);
        public float scale = 1f;
        public float radius = 10f;
        public float rotation = 0f;
        public float assignRotation = 0f;
        public float health = 5f;
        public float hitspeed = 2f;
        public float lasthit = 0f;

        public int id = 0;
        public float querytime = 0f;
        public bool active = true;

        public bool jumped = false;
        public float jumpedStart = 0f;

        public void init()
        {
            animationController = new AnimationController(model.SkeletonBones);

            animationController.StartClip(model.AnimationClips.Values[activeAnimationClipIndex]);
        }

        public void updatePosition()
        {
            if (Math.Floor(assignPosition.X) > Math.Floor(position.X))
            {
                position.X++;
            }
            else if (Math.Floor(assignPosition.X) < Math.Floor(position.X))
            {
                position.X--;
            }

            if (Math.Floor(assignPosition.Y) > Math.Floor(position.Y))
            {
                position.Y++;
            }
            else if (Math.Floor(assignPosition.Y) < Math.Floor(position.Y))
            {
                position.Y--;
            }

            if (Math.Floor(assignPosition.Z) > Math.Floor(position.Z))
            {
                position.Z++;
            }
            else if (Math.Floor(assignPosition.Z) < Math.Floor(position.Z))
            {
                position.Z--;
            }

            // rotation
            if (Math.Floor(assignRotation) > Math.Floor(rotation))
            {
                rotation = rotation + 0.01f;
            }
            else if (Math.Floor(assignRotation) < Math.Floor(rotation))
            {
                rotation = rotation - 0.01f;
            }
        }

        public void updateAnimation(int index)
        {
            if (index != activeAnimationClipIndex)
            {
                index = activeAnimationClipIndex;
                animationController.StartClip(model.AnimationClips.Values[index]);
            }
        }
    }
}
