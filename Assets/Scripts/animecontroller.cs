using UnityEngine;

public class animecontroller : MonoBehaviour
{
    public Animator anim;
     public string targetWord = "5";
    private string currentInput = "";

    private bool hasMoved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
         if (hasMoved) return; // ✅ 如果已经移动过了，就不再处理输入

        foreach (char c in Input.inputString)
        {
            if (char.IsLetterOrDigit(c))
            {
                currentInput += c;

                if (targetWord.StartsWith(currentInput))
                {
                    if (currentInput == targetWord)
                    {
                        anim.Play("babymove");
                        currentInput = "";
                        hasMoved = true; // ✅ 记得标记为已完成
                    }
                }
                else
                {
                    currentInput = "";
                }
            }
        }
    }
}
