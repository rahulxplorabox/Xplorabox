using System;

[Serializable]
public class Positions
{
    public string[] right;
    public string[] wrong;
}

[Serializable]
public class SubCategoryData
{
    public int id;
    public string title;
    public string _script;
    public string type;
    public string url;
    public string thumbnail;
    public Positions positions;
    public string[] title_lang;
    public string[] text_lang;
    public string[] title_lang_url;
    public string[] text_lang_url;
}
[Serializable]
public class CategoryData
{
    public string[] sub_category;
    public int sub_category_id;
    public int category_id;
    public SubCategoryData[] sub_category_data;
}
[Serializable]
public class Root
{
    public string[] category;
    public CategoryData[] category_data;
}
[Serializable]
public class RootObject
{
    public Root[] root;
}