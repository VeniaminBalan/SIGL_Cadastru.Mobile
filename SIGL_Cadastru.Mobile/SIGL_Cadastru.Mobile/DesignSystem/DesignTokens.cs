namespace SIGL_Cadastru.Mobile.DesignSystem;

public static class DesignTokens
{
    // ==================== COLORS ====================
    public static class Colors
    {
        // Primary Brand Colors
        public static Color Primary => Color.FromArgb("#0066CC");
        public static Color PrimaryDark => Color.FromArgb("#004C99");
        public static Color PrimaryLight => Color.FromArgb("#3385DB");

        // Secondary Colors
        public static Color Secondary => Color.FromArgb("#FF6B35");
        public static Color SecondaryDark => Color.FromArgb("#CC5529");
        public static Color SecondaryLight => Color.FromArgb("#FF8B61");

        // Neutral Colors
        public static Color Background => Color.FromArgb("#FFFFFF");
        public static Color Surface => Color.FromArgb("#F5F5F5");
        public static Color Border => Color.FromArgb("#E0E0E0");
        public static Color Divider => Color.FromArgb("#BDBDBD");

        // Text Colors
        public static Color TextPrimary => Color.FromArgb("#212121");
        public static Color TextSecondary => Color.FromArgb("#757575");
        public static Color TextDisabled => Color.FromArgb("#BDBDBD");
        public static Color TextOnPrimary => Color.FromArgb("#FFFFFF");

        // Semantic Colors
        public static Color Success => Color.FromArgb("#4CAF50");
        public static Color Warning => Color.FromArgb("#FF9800");
        public static Color Error => Color.FromArgb("#F44336");
        public static Color Info => Color.FromArgb("#2196F3");
    }

    // ==================== TYPOGRAPHY ====================
    public static class Typography
    {
        // Font Sizes
        public const double FontSizeXxs = 10;
        public const double FontSizeXs = 12;
        public const double FontSizeSm = 14;
        public const double FontSizeMd = 16;
        public const double FontSizeLg = 18;
        public const double FontSizeXl = 20;
        public const double FontSize2xl = 24;
        public const double FontSize3xl = 30;
        public const double FontSize4xl = 36;

        // Font Weights (MAUI uses FontAttributes)
        public static FontAttributes Regular => FontAttributes.None;
        public static FontAttributes Bold => FontAttributes.Bold;
        public static FontAttributes Italic => FontAttributes.Italic;
    }

    // ==================== SPACING ====================
    public static class Spacing
    {
        public const double None = 0;
        public const double Xxs = 2;
        public const double Xs = 4;
        public const double Sm = 8;
        public const double Md = 12;
        public const double Lg = 16;
        public const double Xl = 20;
        public const double Xxl = 24;
        public const double Xxxl = 32;
        public const double Xxxxl = 40;
    }

    // ==================== BORDER RADIUS ====================
    public static class BorderRadius
    {
        public const double None = 0;
        public const double Sm = 4;
        public const double Md = 8;
        public const double Lg = 12;
        public const double Xl = 16;
        public const double Full = 999; // Fully rounded (pill shape)
    }

    // ==================== SHADOWS ====================
    public static class Shadows
    {
        public static Shadow Small => new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
            Offset = new Point(0, 1),
            Radius = 2,
            Opacity = 0.25f
        };

        public static Shadow Medium => new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
            Offset = new Point(0, 2),
            Radius = 4,
            Opacity = 0.25f
        };

        public static Shadow Large => new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#40000000")),
            Offset = new Point(0, 4),
            Radius = 8,
            Opacity = 0.25f
        };
    }

    // ==================== LAYOUT ====================
    public static class Layout
    {
        public const double PagePadding = 20;
        public const double SectionSpacing = 24;
        public const double ComponentSpacing = 16;
        public const double ItemSpacing = 8;
        public const double MaxContentWidth = 1200;
    }

    // ==================== ANIMATION ====================
    public static class Animation
    {
        public const uint DurationFast = 150;
        public const uint DurationNormal = 300;
        public const uint DurationSlow = 500;
    }
}
