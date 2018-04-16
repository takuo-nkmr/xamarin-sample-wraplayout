using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ImageWrapLayout
{
	public class WrapLayout : Layout<View>
	{
		public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(
			"ColumnSpacing",
			typeof(double),
			typeof(WrapLayout),
			2.0,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				((WrapLayout)bindable).InvalidateLayout();
			});

		public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(
			"RowSpacing",
			typeof(double),
			typeof(WrapLayout),
			2.0,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				((WrapLayout)bindable).InvalidateLayout();
			});

		public double ColumnSpacing
		{
			set { SetValue(ColumnSpacingProperty, value); }
			get { return (double)GetValue(ColumnSpacingProperty); }
		}

		public double RowSpacing
		{
			set { SetValue(RowSpacingProperty, value); }
			get { return (double)GetValue(RowSpacingProperty); }
		}

        /// <summary>
        /// 対象のレイアウトを格納するBOXのサイズ（width,height）を設定する。
        /// </summary>
        /// <returns>The measure.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
            // 対象要素のheightの設定
            // widthはwidthConstraintの値
            // ※実質1行の高さは同じなので、高さは(1行の高さ*行数)+((行数-1)*スペース) となる
            double width  = widthConstraint;

            int countChildren = Children.Count;
            int countRows = 1;
            double tmpWidthPerRow = 0;
            double? heightPerRow = null;
            foreach (View child in Children)
            {
                // 子要素のサイズを取得
                SizeRequest childSizeRequest = child.Measure(Double.PositiveInfinity, Double.PositiveInfinity);

                // 1行の幅が全体幅を上回れば行を追加
                tmpWidthPerRow += childSizeRequest.Request.Width;
                if (tmpWidthPerRow >= width)
                {
                    // 1行幅のリセット
                    tmpWidthPerRow = childSizeRequest.Request.Width;

                    // 行を1行追加
                    countRows++;
                }

                // 1行の高さを保存
                if (!heightPerRow.HasValue)
                {
                    heightPerRow = childSizeRequest.Request.Height;
                }
            }

            // 高さの設定
            double height = (double)heightPerRow * countRows + RowSpacing * (countRows - 1);

            return new SizeRequest(new Size(width, height));
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
            double xChild = x;
            double yChild = y;
            double tmpWidthPerRow = 0;
            foreach (View child in Children)
            {
                // 子要素のサイズを取得
                SizeRequest childSizeRequest = child.Measure(Double.PositiveInfinity, Double.PositiveInfinity);

                // 1行の幅が全体幅を上回れば次の行で表示
                tmpWidthPerRow += childSizeRequest.Request.Width;
                if (tmpWidthPerRow >= width)
                {
                    tmpWidthPerRow = childSizeRequest.Request.Width;
                    xChild = x;
                    yChild += RowSpacing + childSizeRequest.Request.Height;
                }

                // カラムの表示位置の設定
                LayoutChildIntoBoundingRegion(child, new Rectangle(new Point(xChild, yChild), new Size(childSizeRequest.Request.Width,childSizeRequest.Request.Height)));

                // カラムのx座標を追加
                xChild += childSizeRequest.Request.Width + ColumnSpacing;
            }
		}
	}
}
