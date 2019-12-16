using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Colors :
//black, 
//gray,  
//white, 
//red,  
//blue, 
//green, 
//yellow,
//orange, 
//brown, 
//purple, 
//pink

public class ColorList
{
    //https://stackoverflow.com/questions/470690/how-to-automatically-generate-n-distinct-colors
    public static readonly List<Color> _kellysMaxContrastSet = new List<Color>
    {
        new Color(0xFF/255.0f,0xB3/255.0f,0x00/255.0f), //Vivid Yellow
        new Color(0x80/255.0f,0x3E/255.0f,0x75/255.0f), //Strong Purple
        new Color(0xFF/255.0f,0x68/255.0f,0x00/255.0f), //Vivid Orange
        new Color(0xA6/255.0f,0xBD/255.0f,0xD7/255.0f), //Very Light Blue
        new Color(0xC1/255.0f,0x00/255.0f,0x20/255.0f), //Vivid Red
        new Color(0xCE/255.0f,0xA2/255.0f,0x62/255.0f), //Grayish Yellow
        new Color(0x81/255.0f,0x70/255.0f,0x66/255.0f), //Medium Gray
    };
}


/*

    OTHER COLORS FOR REFERENCE - less color blind accessible


    UIntToColor( 007D34), //Vivid Green
    UIntToColor( F6768E), //Strong Purplish Pink
    UIntToColor( 00538A), //Strong Blue
    UIntToColor( FF7A5C), //Strong Yellowish Pink
    UIntToColor( 53377A), //Strong Violet
    UIntToColor( FF8E00), //Vivid Orange Yellow
    UIntToColor( B32851), //Strong Purplish Red
    UIntToColor( F4C800), //Vivid Greenish Yellow
    UIntToColor( 7F180D), //Strong Reddish Brown
    UIntToColor( 93AA00), //Vivid Yellowish Green
    UIntToColor( 593315), //Deep Yellowish Brown
    UIntToColor( F13A13), //Vivid Reddish Orange
    UIntToColor( 232C16), //Dark Olive Green

 */
