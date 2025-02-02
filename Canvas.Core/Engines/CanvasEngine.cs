using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.EngineSpace
{
  public class CanvasEngine : Engine, IEngine
  {
    protected SKPaint _penBox = null;
    protected SKPaint _penLine = null;
    protected SKPaint _penShape = null;
    protected SKPaint _penCircle = null;
    protected SKPaint _penCaption = null;
    protected SKPaint _penCaptionShape = null;
    protected SKPathEffect _dotLine = null;
    protected SKPathEffect _dashLine = null;
    protected SKPath _curve = null;

    /// <summary>
    /// Bitmap
    /// </summary>
    public virtual SKBitmap Map { get; protected set; }

    /// <summary>
    /// Canvas
    /// </summary>
    public virtual SKCanvas Canvas { get; protected set; }

    /// <summary>
    /// Instance
    /// </summary>
    /// <returns></returns>
    public override IEngine Instance
    {
      get
      {
        if (Map is null || Canvas is null)
        {
          return null;
        }

        return this;
      }
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override IEngine Create(double index, double value)
    {
      Dispose();

      _curve = new SKPath();
      _dotLine = SKPathEffect.CreateDash(new float[] { 1, 3 }, 0);
      _dashLine = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0);

      _penLine = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Stroke,
        FilterQuality = SKFilterQuality.Low,
        StrokeWidth = 1,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penCircle = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.Low,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penBox = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.Low,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penShape = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.Low,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      _penCaption = new SKPaint
      {
        Color = SKColors.Black,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.Low,
        TextSize = 10,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      };

      _penCaptionShape = new SKPaint
      {
        Color = SKColors.Black,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.Low,
        TextSize = 10,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      X = index;
      Y = value;

      Map = new SKBitmap((int)index, (int)value);
      Canvas = new SKCanvas(Map);

      return this;
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateLine(IList<DataModel> coordinates, ComponentModel shape)
    {
      _penLine.Color = shape.Color;
      _penLine.StrokeWidth = (float)shape.Size;

      switch (shape.Composition)
      {
        case CompositionEnum.Dots: _penLine.PathEffect = _dotLine; break;
        case CompositionEnum.Dashes: _penLine.PathEffect = _dashLine; break;
      }

      Canvas.DrawLine(
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)coordinates[1].X,
        (float)coordinates[1].Y,
        _penLine);
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(DataModel coordinate, ComponentModel shape)
    {
      _penCircle.Color = shape.Color;

      Canvas.DrawCircle(
      (float)coordinate.X,
      (float)coordinate.Y,
      (float)shape.Size,
      _penCircle);
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<DataModel> coordinates, ComponentModel shape)
    {
      _penBox.Color = shape.Color;

      Canvas.DrawRect(
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)(coordinates[1].X - coordinates[0].X),
        (float)(coordinates[1].Y - coordinates[0].Y),
        _penBox);
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<DataModel> coordinates, ComponentModel shape)
    {
      var origin = coordinates.ElementAtOrDefault(0);

      _penShape.Color = shape.Color;

      _curve.Reset();
      _curve.MoveTo((float)origin.X, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        _curve.LineTo((float)coordinates[i].X, (float)coordinates[i].Y);
      }

      Canvas.DrawPath(_curve, _penShape);
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateCaption(DataModel coordinate, ComponentModel shape, string content)
    {
      _penCaption.Color = shape.Color;
      _penCaption.TextSize = (float)shape.Size;
      _penCaption.TextAlign = SKTextAlign.Center;

      switch (shape.Position)
      {
        case PositionEnum.L: _penCaption.TextAlign = SKTextAlign.Left; break;
        case PositionEnum.R: _penCaption.TextAlign = SKTextAlign.Right; break;
      }

      var space = (_penCaption.FontSpacing - _penCaption.TextSize) / 2;

      Canvas.DrawText(
        content,
        (float)coordinate.X,
        (float)(coordinate.Y - space),
        _penCaption);
    }

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateCaptionShape(IList<DataModel> coordinates, ComponentModel shape, string content)
    {
      var origin = coordinates.ElementAtOrDefault(0);

      _penCaptionShape.Color = shape.Color;
      _penCaptionShape.TextSize = (float)shape.Size;

      _curve.Reset();
      _curve.MoveTo((float)origin.X, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        _curve.LineTo((float)coordinates[i].X, (float)coordinates[i].Y);
      }

      _penCaptionShape.Color = shape.Color;
      _penCaptionShape.TextSize = (float)shape.Size;

      Canvas.DrawTextOnPath(content, _curve, 0, _penCaptionShape.TextSize / 2, _penCaptionShape);
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public override DataModel GetContentMeasure(string content, double size)
    {
      _penCaption.TextSize = (float)size;

      var item = new DataModel
      {
        X = content.Length * Math.Min(_penCaption.FontMetrics.MaxCharacterWidth, size),
        Y = _penCaption.FontSpacing
      };

      return item;
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      Canvas.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Encode as image
    /// </summary>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public override byte[] Encode(SKEncodedImageFormat imageType, int quality)
    {
      using (var image = Map.Encode(imageType, quality))
      {
        return image.ToArray();
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      Map?.Dispose();
      Canvas?.Dispose();

      _dotLine?.Dispose();
      _dashLine?.Dispose();
      _penLine?.Dispose();
      _penCircle?.Dispose();
      _penBox?.Dispose();
      _penShape?.Dispose();
      _penCaption?.Dispose();
      _penCaptionShape?.Dispose();

      Map = null;
      Canvas = null;
    }
  }
}
