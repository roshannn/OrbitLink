using UnityEngine;
using UnityEngine.UI;
using Zenject;
using OrbitLink.Data;
using OrbitLink.Services;

namespace OrbitLink.Presentation
{
    /// <summary>
    /// Listens to core logic events and updates standard UI Text elements.
    /// Completely read-only observer of the simulation state.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Inject] private GameSession _session;
        [Inject] private ShipSystem _shipSystem;

        [SerializeField] private Text _walletText;
        [SerializeField] private Text _jamWarningText;

        private void Start()
        {
            // Initializing logic if needed
        }

        private void LateUpdate()
        {
            if (_session == null) return;

            if (_walletText != null)
            {
                // Idle games standard: F0 or custom formating for billions/trillions
                _walletText.text = $"${_session.State.WalletBalance:F0}";
            }

            // Check if any ship is jammed for a global warning
            bool jamDetected = false;
            var ships = _shipSystem?.ActiveShips;
            int count = _shipSystem?.ActiveShipCount ?? 0;

            if (ships != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (ships[i].IsJammed)
                    {
                        jamDetected = true;
                        break;
                    }
                }
            }

            if (_jamWarningText != null)
            {
                _jamWarningText.gameObject.SetActive(jamDetected);
            }
        }
    }
}
