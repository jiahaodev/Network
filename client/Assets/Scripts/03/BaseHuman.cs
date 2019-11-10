/****************************************************
    文件：BaseHuman.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 18:11:38
	功能：BaseHuman
*****************************************************/

using UnityEngine;

public class BaseHuman : MonoBehaviour 
{
    //是否正在移动
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected float attackTime = float.MinValue;

    //移动目标点
    private Vector3 targetPosition;
    //移动速度
    public float speed = 1.2f;
    //动画组件
    private Animator animator;
    //描述
    public string desc = "";

    //移动到某处
    public void MoveTo(Vector3 pos) {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    //移动Update
    public void MoveUpdate()
    {
        if (isMoving == false)
        {
            return;
        }

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    //攻击动作
    public void Attack() {
        isAttacking = true;
        attackTime = Time.time; //记录攻击动作的开始时间
        animator.SetBool("isAttacking",true);
    }

    //攻击Update
    public void AttackUpdate() {
        if (!isAttacking) return;
        if (Time.time - attackTime < 1.2f) return;  //攻击动作未结束
        isAttacking = false;
        animator.SetBool("isAttacking",false);
    }



    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }
}