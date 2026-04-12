<script lang="ts">
    import { GitHub, GlobeOutline } from '~/lib/components/icons';
    import LogoType from '~/lib/components/LogoType.svelte';
    import { button } from '~/lib/utils/styles';
</script>

<footer class="border-t-surface-border relative w-full border-t p-8">
    <LogoType />
    <p class="mt-1 c-help-text">
        Public-first task management built for transparency and collaboration.
    </p>
    <div class="mt-8 flex items-center gap-2">
        <a
            href="https://fullest-web.onrender.com"
            target="_blank"
            rel="noopener noreferrer"
            class="{button({ filled: true, icon: true })} inline-block"
        >
            <GlobeOutline />
        </a>
        <a
            href="https://github.com/duydang2311/fullest-web"
            target="_blank"
            rel="noopener noreferrer"
            class="{button({ filled: true, icon: true })} inline-block"
        >
            <GitHub />
        </a>
    </div>
    <p class="mt-2 text-fg-muted text-sm">© 2025 Fullest, Inc.</p>
</footer>
