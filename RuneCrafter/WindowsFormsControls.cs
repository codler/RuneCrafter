using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Diagnostics;

namespace RuneCrafter
{
    class WindowsFormsControls : GameComponent
    {
        // The form beneath XNA's Game framework
        Form windowsGameForm;
        // Hooks to the graphics device
        IGraphicsDeviceService graphicsService;
        GraphicsDevice graphicsDevice;

        // This section was straight copied from TemplateForm.Designer.cs
        // I'd usually remove the redundant "System.Windows.Forms" namespace declarations
        // in favour of a "using" statement... but that's up to you, both work :)
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.Panel renderPanel;
        private System.Windows.Forms.GroupBox buttonsGroup;
        private System.Windows.Forms.ToolStrip toolBar;


        public WindowsFormsControls(Game game) : base(game) { }

        public override void Initialize()
        {
            // A reference to the graphics service is mandatory since we need to get the graphics device after it's destroyed
            graphicsService = Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            graphicsDevice = graphicsService.GraphicsDevice;

            // Those three events are necessary to keep a "fresh" state, see individual methods
            graphicsService.DeviceCreated += delegate { OnDeviceCreated(); };
            graphicsService.DeviceResetting += delegate { OnDeviceResetting(); };
            graphicsService.DeviceReset += delegate { OnDeviceReset(); };

            // Here's the trick... We know it's a form, so let's cast it as a form!
            // No need to say that this won't work on the Xbox 360...
            windowsGameForm = Control.FromHandle(Game.Window.Handle) as Form;

            // After, we add up our own components to it
            InitializeComponent();
            Game.Window.Title = "nu";
            
            // We can then map events to the components like in a normal Windows Forms context
            //someButton.Click += new EventHandler(someButton_Click);
            //quitMenuItem.Click += delegate { Game.Exit(); };
            
            // And force a reset so that we set the right target to begin with
            graphicsDevice.Reset();

            base.Initialize();
        }

        /// <summary>
        /// The event handler for the someButton's Click event.
        /// </summary>
        void someButton_Click(object sender, EventArgs e)
        {
            Game.Window.Title = "reerberbre";
            System.Windows.Forms.MessageBox.Show("Hej");
            //statusBarItem.Text = "The button has been clicked.";
        }

        /// <summary>
        /// This method is copied from TemplateForm.Designer.cs with a few modifications :
        /// - Some references to "this" like SuspendLayout, Controls.Add and some others actually refer to the
        ///   form, which means you need to change it "this" to "windowsGameForm" which is the actual form here.
        /// - The "ClientSize" property must not be set manually, it causes size problems with XNA's stuff
        /// 
        /// If you use resources (like button images) in your form, you might want to move the resources to
        /// the project instead of leaving them in the template form so that it doesn't depend on it. 
        /// This requires a bit more code changes, which I haven't done here.
        /// 
        /// There is a LOT of redundant code in here but no surprise, it's generated code. You can fix it if you
        /// like your code clean.
        /// </summary>
        void InitializeComponent()
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateForm));
            this.button1 = new System.Windows.Forms.Button();
            windowsGameForm.SuspendLayout();    /* This line was modified */
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.someButton_Click);
            // 
            // Form1
            // 
            windowsGameForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            windowsGameForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.ClientSize = new System.Drawing.Size(729, 456);
            windowsGameForm.Controls.Add(this.button1);
            windowsGameForm.Name = "Form1";
            windowsGameForm.Text = "Form1";
            windowsGameForm.ResumeLayout(false);

        }

        /// <summary>
        /// This is needed for multi-screen setups, where the device is killed and reset
        /// whenever the window is tossed from one screen to another. Probably other situations 
        /// would cause the device to be re-created too, so make sure you have it.
        /// </summary>
        void OnDeviceCreated()
        {
            graphicsDevice = graphicsService.GraphicsDevice;
            graphicsDevice.Reset();
        }

        /// <summary>
        /// The device is reset everytime the window is resized; but we need to tell the graphics
        /// device about the new size since we're in control of it now.
        /// </summary>
        void OnDeviceResetting()
        {
            //graphicsDevice.PresentationParameters.DeviceWindowHandle = renderPanel.Handle;
            //graphicsDevice.PresentationParameters.BackBufferWidth = renderPanel.Width;
            //graphicsDevice.PresentationParameters.BackBufferHeight = renderPanel.Height;

            // This fixes a background painting issue
            windowsGameForm.Invalidate();
        }

        /// <summary>
        /// It's not actually visible, but the render panel is larger than it looks. So we need to
        /// crop the viewport at its actual visible size for screen-space calculations to work.
        /// </summary>
        private void OnDeviceReset()
        {
            //Viewport viewport = graphicsDevice.Viewport;
            //viewport.Width -= buttonsGroup.Width;
            //viewport.Height -= statusBar.Height + menuBar.Height + toolBar.Height;
            //graphicsDevice.Viewport = viewport;
        }
    }
}


