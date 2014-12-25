using System;

public class IMobileIconParams
{
    public IMobileIconParams ()
    {
        iconNumber = 4;
        iconViewLayoutWidth = -1;
		iconSize = -1;
        iconTitleEnable = true;
		iconTitleFontSize = -1;
        iconTitleFontColor = "#FFFFFF";
		iconTitleOffset = -1;
        iconTitleShadowEnable = true;
        iconTitleShadowColor = "#000000";
        iconTitleShadowDx = -1;
        iconTitleShadowDy = -1;
    }

    public int iconNumber{ get; set; }
    public int iconViewLayoutWidth{ get; set; }
	public int iconSize{ get; set; }
    public bool iconTitleEnable{ get; set; }
	public int iconTitleFontSize{ get; set; }
    public string iconTitleFontColor{ get; set; }
	public int iconTitleOffset{ get; set; }
    public bool iconTitleShadowEnable{ get; set; }
    public string iconTitleShadowColor{ get; set; }
    public int iconTitleShadowDx{ get; set; }
    public int iconTitleShadowDy{ get; set; }
}