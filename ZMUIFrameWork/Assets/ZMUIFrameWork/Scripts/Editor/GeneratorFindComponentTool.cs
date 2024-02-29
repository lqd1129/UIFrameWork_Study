using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text;

public class GeneratorFindComponentTool : Editor
{
    public static Dictionary<int, string> objFindPathDic;//key ��ʾ�����insid ��value ��������Ĳ���·��
    public static List<EditorObjectData> objDataList;//���Ҷ��������

    [MenuItem("GameObject/����������ҽű�",false,0)]
    static void CreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject; //��ȡ����ǰѡ�������
        if(obj == null)
        {
            Debug.LogError("��Ҫѡ�� GameObject");
            return;
        }
        objDataList = new List<EditorObjectData>();
        objFindPathDic = new Dictionary<int, string>();

        //���ýű�����·��
        if(!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
        }
        //���������������
        PresWindowNodeData(obj.transform, obj.name);
        //����cs�ű�
        string csContnet = CreateCS(obj.name);
        //Debug.Log("CsConent:\n" + csContnet);
        string cspath = GeneratorConfig.FindComponentGeneratorPath + "/" + obj.name + "UIComponent.cs";
        //���ɽű��ļ�
        if(File.Exists(cspath))
        {
            File.Delete(cspath);
        }
        StreamWriter writer = File.CreateText(cspath);
        writer.Write(csContnet);
        writer.Close();
        AssetDatabase.Refresh();
        //foreach (var item in objDataList)
        //{
        //    Debug.Log( $"fieldNmae: {item.fieldName}");
        //    Debug.Log( $"fieldType: {item.fieldType}");
        //}
        //foreach (var item in objFindPathDic)
        //{
        //    Debug.Log( "����·��" + item.Value);
        //}
    }

    /// <summary>
    /// �������ڽڵ�����
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="wndName"></param>
    public static void PresWindowNodeData(Transform trans , string wndName)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string name = obj.name;
            if (name.Contains("[")&& name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1 ;
                string fieldName = name.Substring(index , name.Length - index); //��ȡ�ֶ��ǳ�
                string fieldType = name.Substring(1, index - 2);//��ȡ�ֶ�����
                objDataList.Add(new EditorObjectData { fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID() });

                //����ýڵ�Ĳ���·��
                string objpath = name;
                bool isFindOver = false;
                Transform parent = obj.transform;
                for (int k = 0; k < 20; k++)
                {
                    parent = parent.parent;
                    //������ڵ��ǵ�ǰ���ڣ�˵�������Ѿ�����
                    if (string.Equals(parent.name, wndName))
                    {
                        isFindOver = true;
                        break;
                    }
                    else
                    {
                        objpath = objpath.Insert(0, parent.name + "/");
                    }
                    if (isFindOver)
                    {
                        break;
                    }
                }
                objFindPathDic.Add(obj.GetInstanceID(),objpath);

            }
            PresWindowNodeData(trans.GetChild(i), wndName);
        }
    }

    public static string CreateCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZMUIFrameWork";
        //�������
        sb.AppendLine("/*---------------------------------");
        sb.AppendLine(" *Title:UI�Զ���������Ҵ������ɹ���");
        sb.AppendLine(" *Author:����");
        sb.AppendLine(" *Date:" + System.DateTime.Now);
        sb.AppendLine(" *Description:������Ҫ��[Text]���ż�������͵ĸ�ʽ����������Ȼ���Ҽ��������塪�� һ������UI������ҽű�����");
        sb.AppendLine(" *ע��:�����ļ����Զ����ɵģ��κ��ֶ��޸Ķ��ᱻ�´����ɸ���,���ֶ��޸ĺ�,���������Զ�����");
        sb.AppendLine("---------------------------------*/");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using UnityEngine;");

        sb.AppendLine();

        //���������ռ�
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }
        sb.AppendLine($"\tpublic class {name + "UIComponent"}");
        sb.AppendLine("\t{");

        //�����ֶ������б� �����ֶ�
        foreach (var item in objDataList)
        {
            sb.AppendLine("\t\tpublic   " + item.fieldType + "  " + item.fieldName + item.fieldType + ";\n");
        }

        //������ʼ������ӿ�
        sb.AppendLine("\t\tpublic  void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t     //�������");
        //���ݲ���·���ֵ� ���ֶ������б�����������Ҵ���
        foreach (var item in objFindPathDic)
        {
            EditorObjectData itemData = GetEditorObjectData(item.Key);
            string relFieldName = itemData.fieldName + itemData.fieldType;

            if (string.Equals("GameObject", itemData.fieldType))
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").gameObject;");
            }
            else if (string.Equals("Transform", itemData.fieldType))
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").transform;");
            }
            else
            {
                sb.AppendLine($"\t\t     {relFieldName} =target.transform.Find(\"{item.Value}\").GetComponent<{itemData.fieldType}>();");
            }
        }
        sb.AppendLine("\t");
        sb.AppendLine("\t");
        sb.AppendLine("\t\t     //����¼���");
        //�õ��߼��� WindowBase => LoginWindow
        sb.AppendLine($"\t\t     {name} mWindow=({name})target;");

        //����UI�¼��󶨴���
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "Click";
                sb.AppendLine($"\t\t     target.AddButtonClickListener({methodName}{type},mWindow.On{methodName}Button{suffix});");
            }
            if (type.Contains("InputField"))
            {
                sb.AppendLine($"\t\t     target.AddInputFieldListener({methodName}{type},mWindow.On{methodName}InputChange,mWindow.On{methodName}InputEnd);");
            }
            if (type.Contains("Toggle"))
            {
                suffix = "Change";
                sb.AppendLine($"\t\t     target.AddToggleClickListener({methodName}{type},mWindow.On{methodName}Toggle{suffix});");
            }
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine("}");
        }
        return sb.ToString();
    }
    public static EditorObjectData GetEditorObjectData(int insid)
    {
        foreach (var item in objDataList)
        {
            if (item.insID == insid)
            {
                return item;
            }
        }
        return null;
    }
    public class EditorObjectData
    {
        public int insID;
        public string fieldName;
        public string fieldType;
    }
}
