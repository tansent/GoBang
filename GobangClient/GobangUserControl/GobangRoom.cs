using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GobangUserControl
{
    /// <summary>
    /// All the properties and events below are companied with the component,
    /// This is component-design mode. The events here does NOT set aside with
    /// the events in using mode
    /// </summary>
    public partial class GobangRoom : UserControl
    {

       //Whether black is sitted
        [Description("Whether black is sitted")]
        public bool HasBlackPlayer    // This is a property
        {
            get
            {
                if (pictureBoxBlack.Image != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)  // if something has invoke this property
                {
                    pictureBoxBlack.Image = Properties.Resources.player;
                }
                else 
                {
                    pictureBoxBlack.Image = null;
                }
            }
        }

        private bool hasWhitePlayer = false; // dispensable ?


        //Whether white is sitted
        [Description("Whether white is sitted")]
        public bool HasWhitePlayer    // This is a property
        {
            get
            {
                if (pictureBoxWhite.Image != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value == true)  // if something has invoke this property
                {
                    pictureBoxWhite.Image = Properties.Resources.player;
                }
                else
                {
                    pictureBoxWhite.Image = null;
                }
            }
        }
       
        //Black's nickname
        [Description("Black's nickname")]
        public string BlackPlayerName  // A label's property(Like "Text") cannot be set directly if invoke in an auxiliary threading, we need an self-defined property to alter those default(Unaltered) ones
        {
            get
            {
                return labelBlack.Text;
            }
            set
            {
                labelBlack.Text = value;
            }
        }

        //White's nickname
        [Description("White's nickname")]
        public string WhitePlayerName
        {
            get
            {
                return labelWhite.Text;
            }
            set
            {
                labelWhite.Text = value;
            }
        }

        private int selectedSide = -1;

        //Represent the users' status
        //-1 stands for empty seats, 0 for black, 1 for white, 2 for observers

        [Description("Represent the users' status, -1 stands for empty seats, 0 for black, 1 for white, 2 for observers")]
        public int SelectedSide
        {
            get
            {
                return selectedSide;
            }
            set
            {
                selectedSide = value;
            }
        }

        //Observers at the table
        public List<string> lookOnUser = new List<string>();      //
        GraphicsPath formShape = new GraphicsPath();



        public GobangRoom()
        {
            InitializeComponent();
            int i = 20;
            int w = this.Width;
            int h = this.Height;
            Point[] points = 
            {
               new Point(i,0), new Point(w-i,0), new Point(w,i), new Point(w,h-i),
               new Point(w-i,h), new Point(i,h), new Point(0,h-i), new Point(0,i)
            };
            formShape.AddPolygon(points);    
        }

        private void GobangRoom_Paint(object sender, PaintEventArgs e)  //This is the event goes with this component, because it is defined in design
        {
            this.Region = new System.Drawing.Region(formShape);   //Region is a property to control the apparent room that would be demostrated
        }

        //When Clicking the black seat
        private void pictureBoxBlack_Click(object sender, EventArgs e)  // define a new event
        {
            if (HasBlackPlayer == false)
            {
                //sitted
                HasBlackPlayer = true;
                SelectedSide = 0;
            }
            else
            {
                //observe
                selectedSide = 2;             // Change later                
            }
            this.OnValidating(new CancelEventArgs());  // invoke the Validating event             // ?
        }

        private void pictureBoxWhite_Click(object sender, EventArgs e)
        {
            if (HasWhitePlayer == false)
            {
                //sitted
                HasWhitePlayer = true;
                SelectedSide = 1;
            }
            else
            {
                //observe
                selectedSide = 2;             // Change later                
            }
            this.OnValidating(new CancelEventArgs());  //
        }



        

    }
}
