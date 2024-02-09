using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace DoorControlDemo.ViewModels
{
    public class ControlDoorOperationsViewModel : ViewModelBase
    {
        // Instance of ControlDoor
        private readonly Models.ControlDoor _controlDoor;

        public ControlDoorOperationsViewModel(Models.User user)
        {
            _controlDoor = new Models.ControlDoor(user);
        }

        // Commands for door operations
        public ICommand OpenDoorCommand => new RelayCommand(OpenDoor);
        public ICommand CloseDoorCommand => new RelayCommand(CloseDoor);
        public ICommand StayOpenCommand => new RelayCommand(StayOpen);
        public ICommand StayCloseCommand => new RelayCommand(StayClose);

        private void OpenDoor()
        {
            _controlDoor.OpenDoor();
            
        }

        private void CloseDoor()
        {
            _controlDoor.CloseDoor();
        }

        private void StayOpen()
        {
            _controlDoor.StayOpen();
        }

        private void StayClose()
        {
            _controlDoor.StayClose();
        }
    }
}
