using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms; 

public interface RepaintAble
{
    void needRepaint();
}

public class GrabRectangle
{
    private readonly Color doNothingColor = Color.Magenta;
    private readonly Color activeColor = Color.LawnGreen;
    Color _color;
    int _lineWidth;
    readonly DashStyle _dashStyle;
    private Rectangle _rectangle;
    public Size _maxSize {get; set;}
    private Point _lastMovePosition;

    List<GrabSquare> _grabSquares = new List<GrabSquare>();
    private readonly Cursor _insideCursor;
    private Cursor _defaultCursor;

    GrabSquare _activeGrabSquare;
    enum GrabMode { DRAG, MOVE, NONE }
    private GrabMode _mode = GrabMode.NONE;
    private GrabMode _mouseOverMode = GrabMode.NONE;

    private RepaintAble _parent;

    class GrabSquare
    {
        public enum Direction { VERTICAL, HORIZONTAL, DIAGONALNWSE, DIAGONALNESW }
        public enum Location { TOPLEFT, TOPMIDDLE, TOPRIGHT, MIDDLELEFT, MIDDLERIGHT, DOWNLEFT, DOWNMIDDLE, DOWNRIGHT }
        public readonly Direction _direction;
        public readonly Location _location;
        public Cursor _cursor { get; }
        private Size _size;
        private Color _color;
        private int _x;
        private int _y;

        public GrabSquare(Direction direction, Location location, Color c)
        {
            _size = new Size(10, 10);
            _color = c;
            _location = location;
            _direction = direction;
            _x = 0;
            _y = 0;

            switch (_direction)
            {
                case Direction.VERTICAL:
                    _cursor = Cursors.SizeNS;
                    break;
                case Direction.HORIZONTAL:
                    _cursor = Cursors.SizeWE;
                    break;
                case Direction.DIAGONALNWSE:
                    _cursor = Cursors.SizeNWSE;
                    break;
                case Direction.DIAGONALNESW:
                    _cursor = Cursors.SizeNESW;
                    break;
            }
        }

        public void draw(Graphics g, Rectangle r)
        {
            int x1 = r.X;
            int y1 = r.Y;
            int x2 = r.X + r.Width;
            int y2 = r.Y + r.Height;

            //Position ausrechnen (innere Griffe).
            switch (_location)
            {
                case Location.TOPLEFT:
                    _x = x1;
                    _y = y1;
                    break;
                case Location.TOPMIDDLE:
                    _x = x1 + (r.Width - _size.Width) / 2;
                    _y = y1;
                    break;
                case Location.TOPRIGHT:
                    _x = x2 - _size.Width;
                    _y = y1;
                    break;
                case Location.MIDDLELEFT:
                    _x = x1;
                    _y = y1 + (r.Height - _size.Height) / 2;
                    break;
                case Location.MIDDLERIGHT:
                    _x = x1 + r.Width - _size.Width;
                    _y = y1 - (_size.Height - r.Height) / 2;
                    break;
                case Location.DOWNLEFT:
                    _x = x1;
                    _y = y2 - _size.Height;
                    break;
                case Location.DOWNMIDDLE:
                    _x = x1 + (r.Width - _size.Width) / 2;
                    _y = y1 + r.Height - _size.Height;
                    break;
                case Location.DOWNRIGHT:
                    _x = x2 - _size.Width;
                    _y = y2 - _size.Height;
                    break;
            }

            //Malen
            using (Brush br = new SolidBrush(_color))
            {
                g.FillRectangle(br, new Rectangle(_x, _y, _size.Width, _size.Height));
            }
        }

        /// <summary>
        /// Test if a  given Point is inside this.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal bool isInside(Point location)
        {
            bool retVal;
            //Console.WriteLine("I am: " + _location + " location in: " + location + " and mine: " + _x + " " + _y);
            if (location.X >= _x && location.X <= _x + _size.Width && location.Y >= _y && location.Y <= _y + _size.Height)
            {
                retVal = true;
                Console.WriteLine("Bingooooooooo");
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        public override string ToString()
        {
            //return base.ToString();
            var sb = new StringBuilder();
            sb.Append(_location).Append(" ").Append(_size).Append(" ").Append(_x).Append(" ").Append(_y).Append("\n");
            return sb.ToString();
        }
    }

    public GrabRectangle(Cursor defaultCursor, Rectangle r, Size maxSize, RepaintAble ra)
    {
        Console.WriteLine("clientsize:" + maxSize);
        _parent = ra;
        _rectangle = r;
        _maxSize = maxSize;
        _color = doNothingColor;
        _lineWidth = 2;
        _dashStyle = DashStyle.DashDot;
        _insideCursor = Cursors.SizeAll;
        _defaultCursor = defaultCursor;
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.DIAGONALNWSE, GrabSquare.Location.TOPLEFT, _color));
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.VERTICAL, GrabSquare.Location.TOPMIDDLE, _color));
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.DIAGONALNESW, GrabSquare.Location.TOPRIGHT, _color));

        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.HORIZONTAL, GrabSquare.Location.MIDDLELEFT, _color));
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.HORIZONTAL, GrabSquare.Location.MIDDLERIGHT, _color));

        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.DIAGONALNESW, GrabSquare.Location.DOWNLEFT, _color));
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.VERTICAL, GrabSquare.Location.DOWNMIDDLE, _color));
        _grabSquares.Add(new GrabSquare(GrabSquare.Direction.DIAGONALNWSE, GrabSquare.Location.DOWNRIGHT, _color));
        
        foreach (var gs in _grabSquares)
        {
            Console.WriteLine(gs.ToString());
        }
    }

    public Rectangle checkBounds(Rectangle toCheck, out bool isValid)
    {
        Console.WriteLine("checkBounds() " + toCheck);
        int x = toCheck.X, y = toCheck.Y, width = toCheck.Width, height = toCheck.Height;        
        isValid = true;

        if (toCheck.X < 0)
        {
            isValid = false;
            x = 0;
        }
        if (toCheck.Y < 0)
        {
            isValid = false;
            y = 0;
        }
        if (toCheck.X > _maxSize.Width)
        {
            isValid = false;
            x = _rectangle.X;
        }
        if (toCheck.Y > _maxSize.Height)
        {
            isValid = false;
            y = _rectangle.Y;
        }
        if (toCheck.Right > _maxSize.Width)
        {
            isValid = false;
            width = _maxSize.Width - toCheck.X;
        }
        if (toCheck.Bottom > _maxSize.Height)
        {
            isValid = false;
            height = _maxSize.Height - toCheck.Y;
        }

        Rectangle r = new Rectangle(x, y, width, height);
        Console.WriteLine(" returning: " + r + " isValid: " + isValid);
        return r;
    }

    public void draw(Graphics g)
    {
        Console.WriteLine("GrabRectangle.draw()");
        foreach (GrabSquare gs in _grabSquares)
        {
            gs.draw(g, _rectangle);
        }
        using (Pen pen = new Pen(_color, _lineWidth))
        {
            //x1: 140 height: 864 x2: 1395 Width 1536 y1: 0 y2: 862
            pen.DashStyle = _dashStyle;
            g.DrawRectangle(pen, _rectangle);
            //const int square = 10;
            //e.Graphics.FillRectangle(brush, new Rectangle(x1, y1, square, square));
            //e.Graphics.FillRectangle(brush, new Rectangle(x2 - square, y1, square, square));
            //e.Graphics.FillRectangle(brush, new Rectangle(x1, y2 - square, square, square));
            //e.Graphics.FillRectangle(brush, new Rectangle(x2 - square, y2 - square, square, square));  
        }
    }

    public Cursor onMouseMove(MouseEventArgs e)
    {
        //Mode: ziehen in eine Richtung oder Rahmen verschieben oder nix
        //schauen ob maus über was drüber ist -> Cursor wechseln

        if (_mode.Equals(GrabMode.NONE))
        {
            Console.WriteLine("_mode.Equals(GrabMode.NONE)");
            Cursor c = checkForActiveRegionChangeCursor(e.Location);
            if (_activeGrabSquare == null && !c.Equals(_insideCursor)) //nix
            {
                _color = doNothingColor;
                return c;
            }
            else   //mode change
            {
                //_color = activeColor;
                return c;  //next move for dragging, moving...
            }
        }
        else if (_mode.Equals(GrabMode.DRAG))
        {
            Console.WriteLine("_mode.Equals(GrabMode.DRAG)" + e.Location + " " + e.X + " " + e.Y);
            _color = activeColor;
            switch (_activeGrabSquare._direction)
            {
                case GrabSquare.Direction.HORIZONTAL:
                    if (_activeGrabSquare._location.Equals(GrabSquare.Location.MIDDLELEFT))
                    {
                        _rectangle.Width = Math.Max(0, _rectangle.Right - Math.Max(0, e.X));                          
                        if (_rectangle.Width > 0)
                            _rectangle.X = Math.Max(0, e.X); 
                    } else
                    {
                        _rectangle.Width = Math.Max(0, e.X - _rectangle.X);
                        if (_rectangle.Right >= _maxSize.Width)
                            _rectangle.Width = _maxSize.Width - _rectangle.X;                  
                    }
                    break;
                case GrabSquare.Direction.VERTICAL:
                    if (_activeGrabSquare._location.Equals(GrabSquare.Location.TOPMIDDLE))
                    {
                        _rectangle.Height = Math.Max(0, _rectangle.Bottom - Math.Max(0, e.Y));
                        if (_rectangle.Height > 0)
                        {
                            _rectangle.Y = Math.Max(0, e.Y);
                        }                   
                    } else
                    {
                        _rectangle.Height = Math.Max(0, e.Y - _rectangle.Y);
                        if (_rectangle.Bottom > _maxSize.Height)
                        {
                            _rectangle.Height = _maxSize.Height - _rectangle.Y;
                        }
                    }
                    break;
                case GrabSquare.Direction.DIAGONALNESW:
                    if (_activeGrabSquare._location.Equals(GrabSquare.Location.TOPRIGHT))
                    {                       
                        _rectangle.Width = Math.Max(0, Math.Min(_maxSize.Width, e.X) - _rectangle.X);
                        _rectangle.Height = Math.Max(0, _rectangle.Y + _rectangle.Height - Math.Max(0, e.Y));
                        if (_rectangle.Height > 0)
                            _rectangle.Y = Math.Max(0, e.Y);   
                    } else
                    {                          
                        _rectangle.Width = Math.Max(0, _rectangle.X + _rectangle.Width - Math.Max(0,e.X));
                        _rectangle.Height = Math.Max(0, Math.Min(_maxSize.Height, e.Y) - _rectangle.Y);
                        if (_rectangle.Bottom > _maxSize.Height)
                            _rectangle.Height = _maxSize.Height - _rectangle.Y;
                        if (_rectangle.Left < 0)
                        {
                            _rectangle.X = 0;                           
                        }
                        if (_rectangle.Width > 0)
                            _rectangle.X = Math.Max(0, e.X);

                    }
                    break;
                case GrabSquare.Direction.DIAGONALNWSE:
                    if (_activeGrabSquare._location.Equals(GrabSquare.Location.TOPLEFT))
                    {
                        _rectangle.Width = Math.Max(0, _rectangle.Right - Math.Max(0, e.X));
                        _rectangle.Height = Math.Max(0, _rectangle.Bottom - Math.Max(0, e.Y));
                        if (_rectangle.Width > 0)
                            _rectangle.X = Math.Max(0, e.X);
                        if (_rectangle.Height > 0)
                            _rectangle.Y = Math.Max(0, e.Y);
                        
                    } else
                    {
                        _rectangle.Width = Math.Max(0, e.X - _rectangle.X);
                        if (_rectangle.Right > _maxSize.Width)
                        {
                            _rectangle.Width = _maxSize.Width - _rectangle.X;
                        }
                        _rectangle.Height = Math.Max(0, e.Y - _rectangle.Y); 
                        if (_rectangle.Bottom > _maxSize.Height)
                        {
                            _rectangle.Height = _maxSize.Height - _rectangle.Y;
                        }
                    }
                    break;                     
            }
            _parent.needRepaint();
            return _activeGrabSquare._cursor;
        } else if (_mode.Equals(GrabMode.MOVE)) {
            Console.WriteLine("_mode.Equals(GrabMode.MOVE)");
            if (e.X > _maxSize.Width || e.X < 0 || e.Y < 0 || e.Y > _maxSize.Height)
            {
                // _mode = GrabMode.NONE;
                _color = doNothingColor;
                _parent.needRepaint();
                return _insideCursor;
            }
            _color = activeColor;
            int x = _rectangle.X;
            int y = _rectangle.Y;
            int eX = Math.Max(0, e.X);
            eX = Math.Min(eX, _maxSize.Width);
            int eY = Math.Max(0, e.Y);
            eY = Math.Min(eY, _maxSize.Height);
             
            _rectangle.X = Math.Max(0, _rectangle.X + (eX - _lastMovePosition.X));
            _rectangle.Y = Math.Max(0, _rectangle.Y + (eY - _lastMovePosition.Y));

            if (_rectangle.Right > _maxSize.Width)
                _rectangle.X = x;
            if (_rectangle.Bottom > _maxSize.Height)
                _rectangle.Y = y;
                
            _lastMovePosition = e.Location;
           
            
            _parent.needRepaint();
            return _insideCursor;
        }  else
        {
            throw new Exception(_mode + " unhandled");  //we test...
        } 
    }

    /// <summary>
    /// Returns a cursor. Sets the _mode. Sets the _activeGraSquare (if there is one).
    /// </summary>
    /// <param name="location">The point to check. (0, 0) is upper left of the form</param>
    /// <returns>An approbiate Cursor or the default Cursor (the cached cursor of the Form)</returns>
    private Cursor checkForActiveRegionChangeCursor(Point location)
    {
        Console.WriteLine("checkForActiveRegionChangeCursor() " + location);
        
        //over Grabsqares?
        GrabSquare gs = null;
        bool isInsideGrabSquare = false;
        for (int i = 0; i < _grabSquares.Count; i++)
        {
            gs = _grabSquares[i];
            isInsideGrabSquare = gs.isInside(location);
            if (isInsideGrabSquare)
            {                   
                break;
            }            
        }

        if (isInsideGrabSquare)
        {
            //change forms cursor
            _activeGrabSquare = gs;
            _mouseOverMode = GrabMode.DRAG;
            return _activeGrabSquare._cursor;
        }         
        //over Rectangle himself?
        if (_rectangle.Contains(location)) {
            _activeGrabSquare = null;
            _lastMovePosition = location;           
            _mouseOverMode = GrabMode.MOVE;
            return _insideCursor;
        } else
        {
            _activeGrabSquare = null;
            _mouseOverMode = GrabMode.NONE;
            return _defaultCursor;
        }
    }

    public void onMouseDown(MouseEventArgs e)
    {
        _lastMovePosition = e.Location; //initialize for move
        _mode = _mouseOverMode;
    }

    public void onMouseUp(MouseEventArgs e)
    {
        _mode = GrabMode.NONE;
        _color = doNothingColor;
        _parent.needRepaint();
    }

    public void setRectangle(Rectangle r)
    {
        _rectangle = r;
    }

    public Rectangle getRectangleAsCopy()
    {        
        return new Rectangle(_rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height);
    }
}
