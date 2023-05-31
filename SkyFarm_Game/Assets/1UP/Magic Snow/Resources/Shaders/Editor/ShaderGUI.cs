using UnityEditor;
using UnityEngine;

namespace GUIAsset1UP
{
    public class MagicSnowGUI : ShaderGUI
    {
        static Texture2D bannerTex = null;
        //static Texture2D logoTex = null;

        static GUIStyle rateTxt = null;
        static GUIStyle title = null;
        static GUIStyle linkStyle = null;
        static GUIStyle horizontalLine;

        static GUIStyle _foldoutStyle;
        static GUIStyle foldoutStyle
        {
            get
            {
                if (_foldoutStyle == null)
                {
                    _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    _foldoutStyle.font = EditorStyles.boldFont;
                }
                return _foldoutStyle;
            }
        }

        static GUIStyle _boxStyle;
        static GUIStyle boxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle(EditorStyles.helpBox);
                }
                return _boxStyle;
            }
        }

        public static bool tessellationFold = false;
        MaterialProperty _Tess;

        public static bool snowFold = false;
        MaterialProperty _SnowColor;
        MaterialProperty _PathColor;
        MaterialProperty _SmoothColor;
        MaterialProperty _SmoothRatio;
        MaterialProperty _SmoothOffset;
        MaterialProperty _SnowDepth;
        MaterialProperty _PathTex;

        public static bool diffuseFold = false;
        MaterialProperty _DiffuseTex;
        MaterialProperty _DiffuseRatio;

        public void FindProperties(MaterialProperty[] props)
        {
            _Tess = FindProperty("_Tess", props);

            _SnowColor = FindProperty("_SnowColor", props);
            _PathColor = FindProperty("_PathColor", props);
            _SmoothColor = FindProperty("_SmoothColor", props);
            _SmoothRatio = FindProperty("_SmoothRatio", props);
            _SmoothOffset = FindProperty("_SmoothOffset", props);
            _SnowDepth = FindProperty("_SnowDepth", props);
            _PathTex = FindProperty("_PathTex", props);

            _DiffuseTex = FindProperty("_DiffuseTex", props);
            _DiffuseRatio = FindProperty("_DiffuseRatio", props, false);
        }

        const int space = 10;

        public override void OnGUI(MaterialEditor materialEditor,
            MaterialProperty[] props)
        {
            FindProperties(props);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(-7);
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            DrawGUI(materialEditor);
            if (EditorGUI.EndChangeCheck())
            {
                var material = (Material)materialEditor.target;
                EditorUtility.SetDirty(material);
                //testHiddenProps.Calculate(material);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(1);
            EditorGUILayout.EndHorizontal();
            //base.OnGUI(materialEditor, props);
        }

        void DrawProperty(MaterialEditor editor, MaterialProperty prop)
        {
            if (editor == null || prop == null) return;
            editor.ShaderProperty(prop, prop.displayName);
        }

        public void DrawGUI(MaterialEditor materialEditor)
        {
            materialEditor.SetDefaultGUIWidths();
            materialEditor.UseDefaultMargins();

            bannerTex = Resources.Load<Texture2D>("banner");
            //logoTex = Resources.Load<Texture2D>("logo");
            if (rateTxt == null)
            {
                rateTxt = new GUIStyle();
                //rateTxt.alignment = TextAnchor.LowerCenter;
                rateTxt.alignment = TextAnchor.LowerRight;
                // rateTxt.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
                rateTxt.normal.textColor = Color.black;
                rateTxt.fontSize = 9;
                rateTxt.padding = new RectOffset(5, 20, 0, 10);
            }
            if (title == null)
            {
                title = new GUIStyle(rateTxt);
                //title.alignment = TextAnchor.UpperCenter;
                title.normal.textColor = new Color(1f, 1f, 1f);
                title.alignment = TextAnchor.MiddleCenter;
                title.fontSize = 19;
                title.padding = new RectOffset(0, 0, 0, 3);
            }

            linkStyle = new GUIStyle();
            if (bannerTex != null)
            {
                GUILayout.Space(5);
                var rect = GUILayoutUtility.GetRect(0, int.MaxValue, 0, 0);
                rect = GUILayoutUtility.GetAspectRect(bannerTex.width / bannerTex.height);
                EditorGUI.DrawPreviewTexture(rect, bannerTex, null, ScaleMode.ScaleAndCrop);
                EditorGUI.LabelField(rect, "V1.0", rateTxt);
                //EditorGUI.LabelField(rect, "1UP Magic Shadow", title);

                if (GUI.Button(rect, "", linkStyle))
                {
                    Application.OpenURL("https://assetstore.unity.com/publishers/7986");
                }
                GUILayout.Space(3);
                if (GUILayout.Button("1UP GAMES ASSET STORE"))
                    Application.OpenURL("https://assetstore.unity.com/publishers/7986");
            }

            tessellationFold = BeginFold("Mesh Subdivision", tessellationFold);
            if (tessellationFold)
            {
                DrawHorizontalLine();
                DrawProperty(materialEditor, _Tess);
            }
            EndFold();

            snowFold = BeginFold("Snow", snowFold);
            if (snowFold)
            {
                DrawHorizontalLine();
                DrawProperty(materialEditor, _SnowColor);
                DrawProperty(materialEditor, _PathColor);
                DrawProperty(materialEditor, _SmoothColor);
                DrawProperty(materialEditor, _SmoothRatio);
                DrawProperty(materialEditor, _SmoothOffset);
                DrawProperty(materialEditor, _SnowDepth);
                //DrawProperty(materialEditor, _PathTex);
            }
            EndFold();

            diffuseFold = BeginFold("Detail Texture", diffuseFold);
            if (diffuseFold)
            {
                DrawHorizontalLine();
                DrawProperty(materialEditor, _DiffuseTex);
                DrawProperty(materialEditor, _DiffuseRatio);
            }
            EndFold();
        }

        public static bool BeginFold(string foldName, bool foldState)
        {
            EditorGUILayout.BeginVertical(boxStyle);
            GUILayout.Space(3);

            EditorGUI.indentLevel++;
            foldState = EditorGUI.Foldout(EditorGUILayout.GetControlRect(),
                foldState, " - " + foldName + " - ", true, foldoutStyle);
            EditorGUI.indentLevel--;

            if (foldState) GUILayout.Space(3);
            return foldState;
        }

        public static void EndFold()
        {
            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(0);
        }

        #region 画线方法
        static void DrawHorizontalLine()
        {
            //画线方法一
            // Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
            // rect.height = lineHeight;
            // EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

            //画线方法二
            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 0, 8);
            horizontalLine.fixedHeight = .5f;
            HorizontalLine(Color.gray);
        }

        static void HorizontalLine(Color color)
        {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

        //画线方法三
        public static void DrawUILine(Color color, float thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        #endregion

        //bool init = false;
        public override void OnMaterialPreviewGUI(MaterialEditor materialEditor, Rect r, GUIStyle background)
        {
            base.OnMaterialPreviewGUI(materialEditor, r, background);
            //  if (init) return;
            //  init = true;
        }

    }
}
