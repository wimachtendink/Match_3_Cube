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
        new Color(0x00/255.0f,0x7D/255.0f,0x34/255.0f),  //Vivid Green
        new Color(0xF6/255.0f,0x76/255.0f,0x8E/255.0f),   //Strong Purplish Pink
        new Color(0x00/255.0f,0x53/255.0f,0x8A/255.0f),  //Strong Blue
        new Color(0xFF/255.0f,0x7A/255.0f,0x5C/255.0f),  //Strong Yellowish Pink
        new Color(0x53/255.0f,0x37/255.0f,0x7A/255.0f),  //Strong Violet
        new Color(0xFF/255.0f,0x8E/255.0f,0x00/255.0f),   //Vivid Orange Yellow
        new Color(0xB3/255.0f,0x28/255.0f,0x51/255.0f),  //Strong Purplish Red
        new Color(0xF4/255.0f,0xC8/255.0f,0x00/255.0f),  //Vivid Greenish Yellow
        new Color(0x7F/255.0f,0x18/255.0f,0x0D/255.0f),  //Strong Reddish Brown
        new Color(0x93/255.0f,0xAA/255.0f,0x00/255.0f),  //Vivid Yellowish Green
        new Color(0x59/255.0f,0x33/255.0f,0x15/255.0f),  //Deep Yellowish Brown
        new Color(0xF1/255.0f,0x3A/255.0f,0x13/255.0f),  //Vivid Reddish Orange
        new Color(0x23/255.0f,0x2C/255.0f,0x16/255.0f),  //Dark Olive Green

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
