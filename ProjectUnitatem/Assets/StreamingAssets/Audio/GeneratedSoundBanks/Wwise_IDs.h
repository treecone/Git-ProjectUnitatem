/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID DEATH = 779278001U;
        static const AkUniqueID ENEMY_ATTACK = 1781417190U;
        static const AkUniqueID MOVE = 3011204530U;
        static const AkUniqueID PAUSE_MENU = 3422541661U;
        static const AkUniqueID PLAY_BGM = 3126765036U;
        static const AkUniqueID PLAY_TESTSOUND = 2752533807U;
        static const AkUniqueID PLAYER_ATTACK = 2824512041U;
        static const AkUniqueID RESUME_GAME = 1565052233U;
        static const AkUniqueID SET_8BITSWITCH = 3901001571U;
        static const AkUniqueID SET_GUITARSWITCH = 4003498308U;
        static const AkUniqueID SET_VIOLINSWITCH = 1399716893U;
        static const AkUniqueID START_GAME = 1114964412U;
        static const AkUniqueID STOP_ALL = 452547817U;
        static const AkUniqueID VICTORY_STINGER = 2155888594U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace PHASES
        {
            static const AkUniqueID GROUP = 3630029033U;

            namespace STATE
            {
                static const AkUniqueID COMBAT = 2764240573U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID TRANSITION = 1865857008U;
            } // namespace STATE
        } // namespace PHASES

        namespace PLAYER_LIFE
        {
            static const AkUniqueID GROUP = 3762137787U;

            namespace STATE
            {
                static const AkUniqueID ALIVE = 655265632U;
                static const AkUniqueID DEAD = 2044049779U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace PLAYER_LIFE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace MUSICSWITCH
        {
            static const AkUniqueID GROUP = 1445037870U;

            namespace SWITCH
            {
                static const AkUniqueID GUITAR = 3232836819U;
                static const AkUniqueID RETRO = 3496907731U;
                static const AkUniqueID VIOLIN = 3063529226U;
            } // namespace SWITCH
        } // namespace MUSICSWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID BGM_CONTROLLER = 3230961072U;
        static const AkUniqueID DAMAGETOBOSS = 1049310662U;
        static const AkUniqueID DISTORTION = 1517463400U;
        static const AkUniqueID PAUSEFILTER = 1201445347U;
        static const AkUniqueID PLAYER_HEALTH = 215992295U;
        static const AkUniqueID SCORE = 2398231425U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN_SOUNDBANK = 2228651116U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID BGM_BUS = 1697111475U;
        static const AkUniqueID ENEMY_ATTACK_BUS = 1664460283U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID PLAYER_ATTACK_BUS = 1578025756U;
        static const AkUniqueID TRANSITIONS_BUS = 228317785U;
        static const AkUniqueID UI_BUS = 3600729941U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
