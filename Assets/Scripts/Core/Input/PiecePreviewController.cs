using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public class PiecePreviewController : MonoBehaviour
    {

        public SpriteRenderer SpriteRenderer;
    
    
        private void Update()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            if (Camera.main != null)
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                world.z = 0;

                transform.position = world;
            }
        }
    
        public void SetPiece(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }
    }
}