using System.Collections;
using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.DrawLine
{
    /// <summary>
    /// ����
    /// �����ˣ�����
    /// </summary>
    public class Drawable : MonoBehaviour
    {
        public Color penColor = Color.red;                  //������ɫ

        [Range(1, 10)]
        public int penWidth = 1;

        public delegate void BrushFunction(Vector2 world_position);
        public BrushFunction currentBrush;                          //���ƺ���

        [HideInInspector]
        public Color resetColour = new Color(0, 0, 0, 0);           //����Ϊ����ɫ
        private Sprite drawableSprite;                                      //���徫����

        private Texture2D drawableTexture;                                  //���徫�����Texture2D
        private Vector2 preDragPosition;                                    //֮ǰ�����ק��λ��

        private Color[] cleanColorArray;                                    //������յ������ɫ����
        private Color32[] curColor;                                         //���徫����ĵ�ǰ��ɫ����(�����Ƕ�̬�ı�)

        private int handIndex = -1;
        private bool IsPressed = false;

        void Awake()
        {
            currentBrush = PenBrush;                //Ĭ�ϱ�ˢ
            drawableSprite = this.GetComponent<SpriteRenderer>().sprite;
            drawableTexture = drawableSprite.texture;

            cleanColorArray = new Color[(int)drawableSprite.rect.width * (int)drawableSprite.rect.height];  //����ʱTexture2D����ɫ

            for (int x = 0; x < cleanColorArray.Length; x++)
                cleanColorArray[x] = resetColour;
        }

        private void OnEnable()
        {
            EventHandGrip.AddListener(OnGrip);
            EventHandIdle.AddListener(OnIdle);

            MSwitchManager.AddListener(OnSwitch);
        }

        private void OnSwitch(OperateModeType operateType)
        {
            if (operateType != OperateModeType.Tool)
                SetClear();
        }

        private void OnGrip(int handIndex)
        {
            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            IsPressed = true;
            this.handIndex = handIndex;
        }

        private void OnIdle(int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsPressed = false;
            preDragPosition = Vector2.zero;
        }

        private void OnDisable()
        {
            EventHandGrip.RemoveListener(OnGrip);
            EventHandIdle.RemoveListener(OnIdle);
            MSwitchManager.RemoveListener(OnSwitch);
        }

        void Update()
        {
            if (IsPressed && MSwitchManager.CurrentMode == OperateModeType.Tool)
            {
                Vector3 temp = MOperateManager.GetHandScreenPoint(handIndex);

                temp.x = Mathf.Clamp(temp.x, 0, Screen.width);
                temp.y = Mathf.Clamp(temp.y, 0, Screen.height);

                Vector2 mouse_world_position = MUtility.UICamera.ScreenToWorldPoint(temp);

                currentBrush(mouse_world_position);
            }
        }

        /// <summary>
        /// ������������ģ��
        /// </summary>
        /// <param name="worldPosition"></param>
        public void BrushTemplate(Vector2 worldPosition)
        {
            // 1. ������λ�ø���Ϊ��������
            Vector2 pixelPos = WorldToPixelCoordinates(worldPosition);

            // 2. ȷ����������ı����ڴ�֡�и���
            curColor = drawableTexture.GetPixels32();

            if (preDragPosition == Vector2.zero)
            {
                MarkPixelsToColour(pixelPos, penWidth, penColor);              //��һ�ε��
            }
            else
            {
                
                ColourBetween(preDragPosition, pixelPos, penWidth, penColor);  //����֮ǰ���λ�ú͵�ǰ���λ�ý��в���
            }
            
            ApplyMarkedPixelChanges();                                          //����ɫӦ�õ���ͼ

            // 4. ����϶�������֮ǰ��λ��
            preDragPosition = pixelPos;
        }

        /// <summary>
        /// ʵʱ���ƺ�������worldPoint��Χ���ظ���Ϊ��̬penColour��ɫ
        /// </summary>
        /// <param name="worldPoint"></param>
        public void PenBrush(Vector2 worldPoint)
        {
            Vector2 pixelPos = WorldToPixelCoordinates(worldPoint);

            curColor = drawableTexture.GetPixels32();

            if (preDragPosition == Vector2.zero)
                MarkPixelsToColour(pixelPos, penWidth, penColor);                  // ����ǵ�һ���϶���ֻ꣬�������λ��Ϊ������ɫ
            else
                ColourBetween(preDragPosition, pixelPos, penWidth, penColor);      // ���ϴθ��µ��õ�λ�ÿ�ʼ��ɫ
            ApplyMarkedPixelChanges();

            preDragPosition = pixelPos;
        }


        //���û��ʰ󶨵Ļ��ƺ���
        public void SetPenBrush()
        {
            currentBrush = PenBrush;
        }

        /// <summary>
        /// ����֮���ֵ����
        /// ������������Ż���ʹ���߸�ƽ��
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        public void ColourBetween(Vector2 startPoint, Vector2 endPoint, int width, Color color)
        {
            float distance = Vector2.Distance(startPoint, endPoint);    //��Ҫ��ֵ�ľ���
            Vector2 direction = (startPoint - endPoint).normalized;     //��ֵ����

            Vector2 curPosition = startPoint;

            float lerp_steps = 1 / distance;                            //��ֵ����

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)         //��ֵ
            {
                curPosition = Vector2.Lerp(startPoint, endPoint, lerp);
                MarkPixelsToColour(curPosition, width, color);
            }
        }

        /// <summary>
        /// ��penThickness���ʰ뾶��Ϊ���İ��������ļ���Χ������Ϊ��Ҫ��ɫ
        /// ����鱻�����ɫ�ĵ��Ƿ񳬳���Χ
        /// </summary>
        /// <param name="centerPixel"></param>
        /// <param name="penThickness"></param>
        /// <param name="penColor"></param>
        public void MarkPixelsToColour(Vector2 centerPixel, int penThickness, Color penColor)
        {
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;

            //����centerPixel��penThickness�����ÿһ��������Ҫ��ɫ��������
            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                if (x >= (int)drawableSprite.rect.width     //����ʱ�򳬳�rect��С
                    || x < 0)
                    continue;

                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    MarkPixelToChange(x, y, penColor);
                }
            }
        }

        /// <summary>
        /// �ѵ�ǰ���ص���ɫ��¼��curColor����
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void MarkPixelToChange(int x, int y, Color color)
        {
            int arrayPos = y * (int)drawableSprite.rect.width + x;

            if (arrayPos > curColor.Length || arrayPos < 0)
                return;

            if (arrayPos >= curColor.Length) return;
            curColor[arrayPos] = color;
        }

        /// <summary>
        /// ����ɫӦ�õ��������Texture2D
        /// </summary>
        public void ApplyMarkedPixelChanges()
        {
            drawableTexture.SetPixels32(curColor);
            drawableTexture.Apply();
        }

        /// <summary>
        /// ֱ��Ϊ������ɫ����ʹ��MarkPixelsToColourȻ��ʹ��ApplyMarkedPixelChanges��
        /// ��ΪSetPixels32��SetPixel��ö�
        /// ��penThickness���ʰ뾶��Ϊ���İ��������ļ���Χ��ɫ
        /// </summary>
        /// <param name="centerPixel"></param>
        /// <param name="penThickness"></param>
        /// <param name="penColor"></param>
        public void ColourPixels(Vector2 centerPixel, int penThickness, Color penColor)
        {
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;
            //int extraRadius = Mathf.Min(0, penThickness - 2);

            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    drawableTexture.SetPixel(x, y, penColor);
                }
            }
            drawableTexture.Apply();
        }

        /// <summary>
        /// ������ھ��������������λ��
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToPixelCoordinates(Vector2 worldPosition)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPosition);

            float pixelWidth = drawableSprite.rect.width;
            float pixelHeight = drawableSprite.rect.height;
            float unitsToPixels = pixelWidth / drawableSprite.bounds.size.x * transform.localScale.x;   //��������ֵ�ı���

            float centeredX = localPos.x * unitsToPixels + pixelWidth / 2;                              //�ھ����������ԭ���¼�������ֵ
            float centeredY = localPos.y * unitsToPixels + pixelHeight / 2;

            Vector2 pixelPos = new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));   //floatתint
            return pixelPos;
        }


        /// <summary>
        /// ���þ�������ɫ����ΪcleanColorArray
        /// </summary>
        public void ResetCanvas()
        {
            drawableTexture.SetPixels(cleanColorArray);
            drawableTexture.Apply();
        }

        private void OnDestroy()
        {
            ResetCanvas();
        }

        public void SetPenWidth(int newWidth)
        {
            penWidth = Mathf.Clamp(newWidth, 1, 5);
        }

        public void SetClear()
        {
            ResetCanvas();
            SetPenBrush();
        }
    }
}