using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;
//System.Diagnostics.Trace.WriteLine(models);

namespace RuneCrafter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        SampleGrid grid;
        
        connect connect = new connect();
        Camera camera = new Camera();
        Input input = new Input();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Texture2D mDottedLine;
        Rectangle mSelectionBox;

        //List<string> models;
        Dictionary<string,string> sKey;
        List<GameObject> model = new List<GameObject>();

        List<Dictionary<string, string>> assets;

        SpriteFont font;

        GameObject tmodel = new GameObject();
        int tindex = -1;

        float deltaFPSTime = 0;

        string[] arguments;

        long lastId;

        public Game1(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            arguments = args;
            if (args.Length > 0)
            {
                
                Window.Title = args[0];
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            mSelectionBox = new Rectangle(50, 50, 100, 100);

            sKey = connect.selectTwoField("SELECT setting_key, setting_value FROM runecrafter_keysettings");
            input.init(sKey);

            //models = connect.selectOneField("SELECT assets_model FROM runecrafter_assets");

            assets = connect.selectFields("SELECT * FROM runecrafter_assets");

            //Components.Add(new WindowsFormsControls(this));
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Set up the reference grid and sample camera
            grid = new SampleGrid();
            grid.GridColor = Color.LimeGreen;
            grid.GridScale = 10.0f;
            grid.GridSize = 32;
            grid.LoadGraphicsContent(graphics.GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Fonts/Courier New");
       
            mDottedLine = Content.Load<Texture2D>("DottedLine");

            foreach (Dictionary<string,string> item in assets)
            {
                GameObject t = new GameObject();
                t.model = Content.Load<SkinnedModel>(item["assets_model"]);

                t.id = int.Parse(item["assets_id"]);

                t.position.X = float.Parse(item["assets_position_x"]);
                t.position.Y = float.Parse(item["assets_position_y"]);
                t.position.Z = float.Parse(item["assets_position_z"]);

                t.assignPosition.X = float.Parse(item["assets_position_x"]);
                t.assignPosition.Y = float.Parse(item["assets_position_y"]);
                t.assignPosition.Z = float.Parse(item["assets_position_z"]);

                t.scale = float.Parse(item["assets_scale"]);
                t.rotation = float.Parse(item["assets_rotation"]);

                t.assignRotation = float.Parse(item["assets_rotation"]);

                t.radius = float.Parse(item["assets_radius"]);

                t.health = float.Parse(item["assets_health"]);
                t.hitspeed = float.Parse(item["assets_hitspeed"]);

                t.init();
                model.Add(t);
            }
            /*
            foreach (string item in models)
            {
                GameObject t = new GameObject();
                t.model = Content.Load<SkinnedModel>(item);

                t.init();
                model.Add(t);
            }*/
            Vector3 randPos = getFreePos(assets);
            //System.Diagnostics.Trace.WriteLine(randPos);
            tmodel.model = Content.Load<SkinnedModel>("Models/biped");
            tmodel.position = new Vector3(0f, 0f, -150f);
            tmodel.scale = 2f;

            tmodel.position = tmodel.assignPosition = tmodel.prevPosition = randPos;
            if (arguments.Length > 0)
            {
                
                tmodel.model = model[int.Parse(arguments[0])].model;
                tmodel.position = model[int.Parse(arguments[0])].position;
                tmodel.prevPosition = model[int.Parse(arguments[0])].position;
                tmodel.assignPosition = model[int.Parse(arguments[0])].position;
                tmodel.scale = model[int.Parse(arguments[0])].scale;
                tmodel.rotation = model[int.Parse(arguments[0])].rotation;
                tmodel.assignRotation = model[int.Parse(arguments[0])].rotation;
                tmodel.id = model[int.Parse(arguments[0])].id;
                model.RemoveAt(int.Parse(arguments[0]));
            } else {

            lastId = connect.insertquery("INSERT INTO `runecrafter_assets` (`assets_model` ,`assets_position_x` ,`assets_position_y` ,`assets_position_z` ,`assets_scale` ,`assets_radius` ,`assets_health` ,`assets_hitspeed`) " +
                                "VALUES (" +
                                "'Models/biped', '" + randPos.X + "', '" + randPos.Y + "', '" + randPos.Z + "', '2', '10', '5', '2'" +
                                ");");
            tmodel.id = Convert.ToInt32( lastId);
             }
            tmodel.init();

            //model[0].position = new Vector3(50f, 0f, 50f);

            //grid requires a projection matrix to draw correctly
            grid.ProjectionMatrix = camera.cameraProjection;

            //Set the grid to draw on the x/z plane around the origin
            grid.WorldMatrix = Matrix.Identity;
            
        }

        public Vector3 getFreePos(List<Dictionary<string, string>> assets)
        {
            Random random = new Random();
            Vector3 position = new Vector3(0f,0f,0f);
            bool noFreeSpot = true;
            double distance;
            float distancex;
            float distancez;
            int expandFreeSpots = 0;
            while (noFreeSpot)
            {
                noFreeSpot = false;
                position.X = random.Next(-150-expandFreeSpots*10, 150+expandFreeSpots*10);
                position.Z = random.Next(-150-expandFreeSpots*10, 150+expandFreeSpots*10);
                foreach (Dictionary<string, string> item in assets)
                {
                    distancex = position.X - float.Parse(item["assets_position_x"]);
                    distancez = position.Z - float.Parse(item["assets_position_z"]);
                    distance = Math.Sqrt(Math.Pow(distancex, 2) + Math.Pow( distancez , 2));
                    if ( distance <= 11)
                    {
                        noFreeSpot = true;
                        expandFreeSpots++;
                    }
                }
            }
            return position;
        }

        /// Empty
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // The time since Update was called last
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            //System.Diagnostics.Trace.WriteLine(elapsed);
            float fps = 1 / elapsed;
            deltaFPSTime += elapsed;
            if (deltaFPSTime > 1)
            {

                Window.Title = "RuneCrafter <" + fps.ToString() + "> FPS ---  BY: HAN LIN YAP / codler@gmail.com";
                deltaFPSTime -= 1;
            }


            float deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;

            input.update();

            KeyboardState k = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (k.IsKeyDown(Keys.Escape))
            {
                connect.close();
                this.Exit();
            }

            if (input.IsPressed("Leftturn"))
            {
                tmodel.rotation += 0.01f;
            }
            if (input.IsPressed("Rightturn"))
            {
                tmodel.rotation -= 0.01f;
            }

            if (input.IsPressed("forward"))
            {
                //tmodel.position += Vector3.Transform(new Vector3(0f, 0f, 1f), Matrix.CreateRotationY(trotation));
                tmodel.position.X += (float)Math.Sin(tmodel.rotation );
                tmodel.position.Z += (float)Math.Cos(tmodel.rotation);
            }
            if (input.IsPressed("backward"))
            {
                tmodel.position.X += -(float)Math.Sin(tmodel.rotation);
                tmodel.position.Z += -(float)Math.Cos(tmodel.rotation);
            }
            if (input.IsPressed("Leftstraf"))
            {
                tmodel.position.X += (float)Math.Cos(-tmodel.rotation);
                tmodel.position.Z += (float)Math.Sin(-tmodel.rotation);
            }
            if (input.IsPressed("Rightstraf"))
            {
                tmodel.position.X += -(float)Math.Cos(-tmodel.rotation);
                tmodel.position.Z += -(float)Math.Sin(-tmodel.rotation);
            }
            if (input.IsPressed("Jump"))
            {
                if (!tmodel.jumped) {
                    tmodel.jumped = true;
                    tmodel.jumpedStart = deltaTime;
                }
            }
            //System.Diagnostics.Trace.WriteLine(Math.Sin(((deltaTime - tmodel.jumpedStart) * GameConstants.jumpSpeed) % MathHelper.Pi));
            if (tmodel.jumped) {
                tmodel.position.Y = (float)Math.Sin(((deltaTime - tmodel.jumpedStart + 0.1f) * GameConstants.jumpSpeed) % MathHelper.Pi) * GameConstants.jumpHeight;
                //System.Diagnostics.Trace.WriteLine(tmodel.position.Y);
                if (Math.Floor( tmodel.position.Y) <= 0f) {
                    tmodel.position.Y = 0f;
                    tmodel.jumped = false;
                }
                
            }
            tmodel.querytime += elapsed;
            System.Diagnostics.Trace.WriteLine(tmodel.querytime);
            if (tmodel.querytime > GameConstants.queryUpdatefrequency)
            {
                assets = connect.selectFields("SELECT * FROM runecrafter_assets");
                tmodel.querytime = 0f;
            }

            /*
            // update
            foreach (GameObject item in model)
            {
                if (lastId != item.id)
                {
                    item.active = false;
                }
            }

            //assets = connect.selectFields("SELECT * FROM runecrafter_assets");
            foreach (Dictionary<string, string> item in assets)
            {
                int id = int.Parse(item["assets_id"]);
                if (lastId != id)
                {

                    // update
                    bool newObject = true;
                    foreach (GameObject item2 in model)
                    {
                        if (item2.id == id)
                        {
                            item2.assignPosition.X = float.Parse(item["assets_position_x"]);
                            item2.assignPosition.Y = float.Parse(item["assets_position_y"]);
                            item2.assignPosition.Z = float.Parse(item["assets_position_z"]);

                            item2.assignRotation = float.Parse(item["assets_rotation"]);
                            newObject = false;
                            item2.active = true;
                        }
                    }

                    // new object
                    if (newObject == true)
                    {
                        GameObject t = new GameObject();
                        t.model = Content.Load<SkinnedModel>(item["assets_model"]);

                        t.id = int.Parse(item["assets_id"]);

                        t.position.X = float.Parse(item["assets_position_x"]);
                        t.position.Y = float.Parse(item["assets_position_y"]);
                        t.position.Z = float.Parse(item["assets_position_z"]);

                        t.assignPosition.X = float.Parse(item["assets_position_x"]);
                        t.assignPosition.Y = float.Parse(item["assets_position_y"]);
                        t.assignPosition.Z = float.Parse(item["assets_position_z"]);

                        t.scale = float.Parse(item["assets_scale"]);
                        t.rotation = float.Parse(item["assets_rotation"]);

                        t.assignRotation = float.Parse(item["assets_rotation"]);

                        t.radius = float.Parse(item["assets_radius"]);

                        t.health = float.Parse(item["assets_health"]);
                        t.hitspeed = float.Parse(item["assets_hitspeed"]);

                        t.init();
                        model.Add(t);
                    }
                }
            }
            
            // delete
            foreach (GameObject item in model)
            {
                if (lastId != item.id)
                {
                    if (item.active == false)
                    {
                        connect.updatequery("DELETE FROM runecrafter_assets WHERE assets_id = " + item.id + " ");
                        assets = connect.selectFields("SELECT * FROM runecrafter_assets");
                        model.Remove(item);
                        break;
                    }
                }
            }*/

            int index = 0;            
            foreach (GameObject item in model)
            {
                
                item.animationController.Update(gameTime.ElapsedGameTime,Matrix.Identity);
                if (CheckForCollisions(tmodel, item))
                {
                    tmodel.position = tmodel.prevPosition;
                    tindex = index;

                    if (Math.Floor(deltaTime) % tmodel.hitspeed == 1 && Math.Floor(deltaTime) != tmodel.lasthit)
                    {
                        item.health--;
                        tmodel.lasthit = (float)Math.Floor(deltaTime);
                    }
                    if (Math.Floor(deltaTime) % item.hitspeed == 1 && Math.Floor(deltaTime) != item.lasthit)
                    {
                        tmodel.health--;
                        item.lasthit = (float)Math.Floor(deltaTime);
                    }

                    if (item.health <= 0) {
                        connect.updatequery("DELETE FROM runecrafter_assets WHERE assets_id = " + item.id + " ");
                        assets = connect.selectFields("SELECT * FROM runecrafter_assets");
                        model.Remove(item);
                        tindex = -1;
                        break;
                    }
                }

                item.updatePosition();

                index++;
            }

            tmodel.animationController.Update(gameTime.ElapsedGameTime,Matrix.Identity);

            // if is moving
            if (tmodel.prevPosition != tmodel.position)
            {
                tmodel.updateAnimation(0);
            }
            else
            {
                tmodel.updateAnimation(1);
            }
            
            tmodel.prevPosition = tmodel.position;

            //if (arguments.Length > 0)
            //{
               if (tmodel.querytime == 0) {
                connect.updatequery("UPDATE runecrafter_assets SET assets_position_x = " + Math.Floor(tmodel.position.X) + ",assets_position_y = " + Math.Floor(tmodel.position.Y) + ",assets_position_z = " + Math.Floor(tmodel.position.Z) + ",assets_rotation = " + Math.Floor(tmodel.rotation) + " WHERE assets_id = " + tmodel.id + " ");
               }
                   //}


            Vector2 mouse = input.mouseMove();

            tmodel.rotation += -mouse.X / 100f;
            camera.height += mouse.Y;
            camera.distance = input.mouseScroll();
            camera.update(tmodel);

            

            grid.ViewMatrix = camera.cameraView;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            //draw the reference grid so it's easier to get our bearings
            grid.Draw();

                   
            

            foreach (ModelMesh modelMesh in tmodel.model.Model.Meshes)
            {
                foreach (SkinnedModelBasicEffect effect in modelMesh.Effects)
                {
                    // Setup camera
                    effect.View = camera.cameraView;
                    effect.Projection = camera.cameraProjection;

                    effect.World = Matrix.CreateRotationY(tmodel.rotation) * Matrix.CreateScale(tmodel.scale) * Matrix.CreateTranslation(tmodel.position);

                    // Set the animated bones to the model
                    effect.Bones = tmodel.animationController.SkinnedBoneTransforms;



                    // OPTIONAL - Configure material
                    effect.Material.DiffuseColor = new Vector3(0.8f);
                    effect.Material.SpecularColor = new Vector3(0.3f);
                    effect.Material.SpecularPower = 8;

                    // OPTIONAL - Configure lights
                    effect.AmbientLightColor = new Vector3(0.1f);
                    effect.LightEnabled = true;
                    effect.EnabledLights = EnabledLights.One;
                    effect.PointLights[0].Color = Vector3.One;
                    effect.PointLights[0].Position = new Vector3(100);

                }

                // Draw a model mesh
                modelMesh.Draw();
            }

            foreach (GameObject item in model)
            {
                DrawModel(item);
            }

            Draw2D();

            base.Draw(gameTime);
        }

        private void Draw2D()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            // Draw Hello World
            string output = "HP: " + tmodel.health; // +Window.Title;
            /*
            output += "\r\n Px" + tmodel.position.X;
            output += "\r\n Py" + tmodel.position.Y;
            output += "\r\n Pz" + tmodel.position.Z;
            output += "\r\n Mx" + model[0].position.X;
            output += "\r\n My" + model[0].position.Y;
            output += "\r\n Mz" + model[0].position.Z;
            */
            // Find the center of the string
            Vector2 FontOrigin = font.MeasureString(output) / 2;
            // Draw the string
            spriteBatch.DrawString(font, output, new Vector2(200, 200), Color.LightGreen,
                0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.End();

            if (tindex > -1)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);


                output = "Enemy HP: " + model[tindex].health;

                // Find the center of the string
                FontOrigin = font.MeasureString(output) / 2;
                // Draw the string
                spriteBatch.DrawString(font, output, new Vector2(200, 230), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

                spriteBatch.End();
            }
            //Begin drawing with the batch
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            /*
            //Draw the horizontal portions of the selection box 
            DrawHorizontalLine(mSelectionBox.Y);
            DrawHorizontalLine(mSelectionBox.Y + mSelectionBox.Height);

            //Draw the verticla portions of the selection box 
            DrawVerticalLine(mSelectionBox.X);
            DrawVerticalLine(mSelectionBox.X + mSelectionBox.Width);
            */
             
            //End the drawing with the batch 
            spriteBatch.End();
        }

        void DrawModel(GameObject item)
        {
            foreach (ModelMesh modelMesh in item.model.Model.Meshes)
            {
                foreach (SkinnedModelBasicEffect effect in modelMesh.Effects)
                {
                    // Setup camera
                    effect.View = camera.cameraView;
                    effect.Projection = camera.cameraProjection;

                    effect.World = Matrix.CreateRotationY(item.rotation) * Matrix.CreateScale(item.scale) * Matrix.CreateTranslation(item.position);

                    // Set the animated bones to the model
                    effect.Bones = item.animationController.SkinnedBoneTransforms;


                    
                    // OPTIONAL - Configure material
                    effect.Material.DiffuseColor = new Vector3(0.8f);
                    effect.Material.SpecularColor = new Vector3(0.3f);
                    effect.Material.SpecularPower = 8;

                    // OPTIONAL - Configure lights
                    effect.AmbientLightColor = new Vector3(0.1f);
                    effect.LightEnabled = true;
                    effect.EnabledLights = EnabledLights.One;
                    effect.PointLights[0].Color = Vector3.One;
                    effect.PointLights[0].Position = new Vector3(100);
                     
                }

                // Draw a model mesh
                modelMesh.Draw();
            }
        }


        bool CheckForCollisions(GameObject c1, GameObject c2)
        {

            //foreach (ModelMesh mesh in c1.model.Model.Meshes)
            //{
                //Matrix transform1 = c1.animationController..SkinnedBoneTransforms[mesh.ParentBone.Index];
                //Vector3 translation1 = transform1.Translation;

                //foreach (ModelMesh otherMesh in c2.model.Model.Meshes)
                //{
                    //Matrix transform2 = c2.animationController.SkinnedBoneTransforms[otherMesh.ParentBone.Index];
                    //Vector3 translation2 = transform2.Translation;

                    BoundingSphere b1 = new BoundingSphere(c1.position, c1.radius);

                    BoundingSphere b2 = new BoundingSphere(c2.position, c2.radius);
                    
                    if (b1.Intersects(b2))
                    {
                        return true;
                    }
                //}
            //}
            return false;
        }

        BoundingSphere CalculateBoundingSphere(Model model)
        {
            BoundingSphere mergedSphere = new BoundingSphere();
            BoundingSphere[] boundingSpheres;
            int index = 0;
            int meshCount = model.Meshes.Count;

            boundingSpheres = new BoundingSphere[meshCount];
            foreach (ModelMesh mesh in model.Meshes)
            {
                boundingSpheres[index++] = mesh.BoundingSphere;
            }

            mergedSphere = boundingSpheres[0];
            if ((model.Meshes.Count) > 1)
            {
                index = 1;
                do
                {
                    mergedSphere = BoundingSphere.CreateMerged(mergedSphere, boundingSpheres[index]);
                    index++;
                } while (index < model.Meshes.Count);
            }
            mergedSphere.Center.Y = 0;
            return mergedSphere;
        }

        private void DrawHorizontalLine(int thePositionY)
        {

            //When the width is greater than 0, the user is selecting an area to the right of the starting point
            if (mSelectionBox.Width > 0)
            {
                //Draw the line starting at the startring location and moving to the right
                for (int aCounter = 0; aCounter <= mSelectionBox.Width - 10; aCounter += 10)
                {
                    if (mSelectionBox.Width - aCounter >= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
            //When the width is less than 0, the user is selecting an area to the left of the starting point
            else if (mSelectionBox.Width < 0)
            {
                //Draw the line starting at the starting location and moving to the left
                for (int aCounter = -10; aCounter >= mSelectionBox.Width; aCounter -= 10)
                {
                    if (mSelectionBox.Width - aCounter <= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, thePositionY, 10, 5), Color.White);
                    }
                }
            }
        }

        private void DrawVerticalLine(int thePositionX)
        {
            //When the height is greater than 0, the user is selecting an area below the starting point
            if (mSelectionBox.Height > 0)
            {
                //Draw the line starting at the starting loctino and moving down
                for (int aCounter = -2; aCounter <= mSelectionBox.Height; aCounter += 10)
                {
                    if (mSelectionBox.Height - aCounter >= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(thePositionX, mSelectionBox.Y + aCounter, 10, 5), new Rectangle(0, 0, mDottedLine.Width, mDottedLine.Height), Color.White, MathHelper.ToRadians(90), new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            //When the height is less than 0, the user is selecting an area above the starting point
            else if (mSelectionBox.Height < 0)
            {
                //Draw the line starting at the start location and moving up
                for (int aCounter = 0; aCounter >= mSelectionBox.Height; aCounter -= 10)
                {
                    if (mSelectionBox.Height - aCounter <= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(thePositionX - 10, mSelectionBox.Y + aCounter, 10, 5), Color.White);
                    }
                }
            }
        }
    }
}
