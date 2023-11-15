using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Quizzes
{
    public class UIQuizQuestionPartType3View : UIQuizQuestionPartBaseView
    {
        [Header("Effects")]
        public Animation anim;
        public GameObject wrongAnswerImage;

        [Header("Broken Parts Options")]
        public List<UnityEngine.UI.Image> brokenParts;

        [Header("Params")]
        [Range(0, 2)]
        public float jumpHeight;
        [Range(0, 10)]
        public float jumpSpeed;
        public float minTimeDelay;
        public float maxTimeDelay;
        [Space(10)]
        public bool jump = true;

        private bool canSlice = false;
        private Vector2 startPos;
        private AudioClip resSoundEffect;

        void OnValidate()
        {
            minTimeDelay = Mathf.Max(minTimeDelay, 0);
            maxTimeDelay = Mathf.Max(maxTimeDelay, 0);

            anim = GetComponent<Animation>();
        }

        public override void LoadView()
        {
            base.LoadView();
        }

        public override void RemoveView()
        {
            base.RemoveView();
        }

        protected override void CorrectAnswer()
        {
            //base.CorrectAnswer();

            SetEffect(true);
        }

        protected override void WrongAnswer()
        {
            //base.WrongAnswer();

            SetEffect(false);
        }

        public void SliceProcess()
        {
            if (canSlice)
            {
                StopCoroutine("JumpProcess");
                canSlice = false;
                SelectPart();
            }
        }

        public void StartJumpProcess()
        {
            startPos = transform.position;
            StartCoroutine("JumpProcess");
        }

        private IEnumerator JumpProcess()
        {
            while (true)
            {
                if (!jump)
                {
                    yield break;
                }

                float timeDelay = Random.Range(minTimeDelay, maxTimeDelay);
                while (timeDelay > 0)
                {
                    yield return new WaitForEndOfFrame();
                    timeDelay -= Time.deltaTime;

                    if (!jump)
                    {
                        yield break;
                    }
                }

                canSlice = true;

                // jump 
                while (Mathf.Sqrt(Mathf.Pow(transform.position.y - jumpHeight, 2)) > 0.02f)
                {
                    transform.position = new Vector2(transform.position.x, Mathf.Lerp(transform.position.y, jumpHeight, Time.deltaTime * jumpSpeed));
                    yield return new WaitForEndOfFrame();
                }

                // fall
                while (Mathf.Sqrt(Mathf.Pow(transform.position.y - startPos.y, 2)) > 0.01f)
                {
                    transform.position = new Vector2(transform.position.x, Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * jumpSpeed * 1.5f * Mathf.Abs(jumpHeight - transform.position.y)).y);
                    yield return new WaitForEndOfFrame();
                }

                transform.position = startPos;
                canSlice = false;
            }
        }

        private void SetEffect(bool win)
        {
            if (anim)
            {
                anim["BoxDestroying"].speed /= 2f;
                anim.Play();
            }

            wrongAnswerImage.SetActive(!win);
        }

        // for animation event
        public void PlaySoundEffect()
        {
        }

        public override IEnumerator ShowCorrectResult()
        {
            yield return new WaitForSeconds(3f); // wait while box falling down

            // set new jump params
            jump = true;
            jumpHeight /= 5f;
            jumpSpeed *= 4f;
            minTimeDelay = maxTimeDelay = 0;
            StartJumpProcess();
            yield return new WaitForSeconds(3f); // jump process
            jump = false; // stop jumping
            yield return new WaitForSeconds(2f);
            SetEffect(true); // destroy box
            yield return new WaitForSeconds(2f);
        }
    }
}